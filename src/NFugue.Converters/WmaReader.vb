Imports System.IO
Imports System.Runtime.InteropServices
Imports System.Runtime.InteropServices.ComTypes
Imports NAudio.Wave
Imports NAudio.WindowsMediaFormat


''' <summary>
''' NAudio reader for WMA Vorbis files,
''' using a Stream that provides uncompressed audio data from any file that
''' can be read using the WMF (WMA, WMV, MP3, MPE, ASF, etc)
''' </summary>
''' <remarks>
''' Written By Yuval Naveh, modified for stream support by Freefall
''' using: http://stackoverflow.com/questions/19421460/wma-audio-stream-to-mp3-stream-using-naudio-c-sharp
''' </remarks>
Friend Class WmaReader
    Inherits WaveStream

    Private m_waveFormat As WaveFormat
    Private m_repositionLock As New Object()
    Private m_wmaStream As WmaStream2

    Public Sub New(ByVal wmaStream As Stream)
        m_wmaStream = New WmaStream2(wmaStream)
        m_waveFormat = m_wmaStream.Format
    End Sub

    ''' <summary>Constructor - Supports opening a WMA file</summary>
    Public Sub New(ByVal wmaFileName As String)
        m_wmaStream = New WmaStream2(wmaFileName)
        m_waveFormat = m_wmaStream.Format
    End Sub

    ''' <summary>
    ''' This is the length in bytes of data available to be read out from the Read method
    ''' (i.e. the decompressed WMA length)
    ''' n.b. this may return 0 for files whose length is unknown
    ''' </summary>
    Public Overrides ReadOnly Property Length() As Long
        Get
            Return m_wmaStream.Length
        End Get
    End Property

    ''' <summary>
    ''' <see cref="WaveStream.WaveFormat"/>
    ''' </summary>
    Public Overrides ReadOnly Property WaveFormat() As WaveFormat
        Get
            Return m_waveFormat
        End Get
    End Property

    ''' <summary>
    ''' <see cref="Stream.Position"/>
    ''' </summary>
    Public Overrides Property Position() As Long
        Get
            Return m_wmaStream.Position
        End Get
        Set(ByVal value As Long)
            SyncLock m_repositionLock
                m_wmaStream.Position = value
            End SyncLock
        End Set
    End Property

    ''' <summary>
    ''' Reads decompressed PCM data from our WMA file.
    ''' </summary>
    Public Overrides Function Read(ByVal sampleBuffer As Byte(), ByVal offset As Integer, ByVal numBytes As Integer) As Integer
        Dim bytesRead As Integer = 0
        SyncLock m_repositionLock
            ' Read PCM bytes from the WMA File into the sample buffer
            bytesRead = m_wmaStream.Read(sampleBuffer, offset, numBytes)
        End SyncLock

        Return bytesRead
    End Function

    ''' <summary>
    ''' Disposes this WaveStream
    ''' </summary>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing Then
            If m_wmaStream IsNot Nothing Then
                m_wmaStream.Close()
                m_wmaStream.Dispose()
                m_wmaStream = Nothing
            End If
        End If
        MyBase.Dispose(disposing)
    End Sub

    Public Class InteropStream
        Implements IStream
        Implements IDisposable
        Public ReadOnly intern As Stream

        Public Sub New(ByVal strm As Stream)
            intern = strm
        End Sub

        Protected Overrides Sub Finalize()
            Try
                Dispose(True)
            Finally
                MyBase.Finalize()
            End Try
        End Sub

        Public Sub Dispose() Implements IDisposable.Dispose
            Dispose(False)
        End Sub

        Protected Sub Dispose(ByVal final As Boolean)
            If final Then
                intern.Dispose()
            End If
        End Sub

        Public Sub Clone(ByRef ppstm As IStream) Implements IStream.Clone
            ppstm = Nothing
        End Sub

        Public Sub Commit(ByVal grfCommitFlags As Integer) Implements IStream.Commit
            intern.Flush()
        End Sub

        ReadOnly buffer As Byte() = New Byte(4095) {}

        Public Sub CopyTo(ByVal pstm As IStream, ByVal cb As Long, ByVal pcbRead As IntPtr, ByVal pcbWritten As IntPtr) Implements IStream.CopyTo
            If pcbRead <> IntPtr.Zero Then
                Marshal.WriteInt32(pcbRead, 0)
            End If
            If pcbWritten <> IntPtr.Zero Then
                Marshal.WriteInt32(pcbWritten, 0)
            End If
        End Sub

        Public Sub LockRegion(ByVal libOffset As Long, ByVal cb As Long, ByVal dwLockType As Integer) Implements IStream.LockRegion
        End Sub

        Public Sub Read(ByVal pv As Byte(), ByVal cb As Integer, ByVal pcbRead As IntPtr) Implements IStream.Read
            Dim rc As Integer = intern.Read(pv, 0, cb)
            If pcbRead <> IntPtr.Zero Then
                Marshal.WriteInt32(pcbRead, rc)
            End If
        End Sub

        Public Sub Revert() Implements IStream.Revert
        End Sub

        Public Sub Seek(ByVal dlibMove As Long, ByVal dwOrigin As Integer, ByVal plibNewPosition As IntPtr) Implements IStream.Seek
            Dim origin As Long = 0
            If dwOrigin = 1 Then
                ' STREAM_SEEK_CUR
                origin = intern.Position
            ElseIf dwOrigin = 2 Then
                ' STREAM_SEEK_END
                origin = intern.Length
            End If

            Dim pos As Long = origin + dlibMove
            intern.Position = pos

            If plibNewPosition <> IntPtr.Zero Then
                Marshal.WriteInt64(plibNewPosition, pos)
            End If
        End Sub

        Public Sub SetSize(ByVal libNewSize As Long) Implements IStream.SetSize
        End Sub

        Public Sub Stat(ByRef pstatstg As System.Runtime.InteropServices.ComTypes.STATSTG, ByVal grfStatFlag As Integer) Implements IStream.Stat
            Dim res = New System.Runtime.InteropServices.ComTypes.STATSTG()

            res.type = 2
            ' STGTY_STREAM
            res.cbSize = intern.Length

            pstatstg = res
        End Sub

        Public Sub UnlockRegion(ByVal libOffset As Long, ByVal cb As Long, ByVal dwLockType As Integer) Implements IStream.UnlockRegion
        End Sub

        Public Sub Write(ByVal pv As Byte(), ByVal cb As Integer, ByVal pcbWritten As IntPtr) Implements IStream.Write
        End Sub

    End Class

    Public Class WmaStream2
        Inherits Stream

        Private interopStrm As InteropStream = Nothing

        Public Sub New(ByVal fileStream As Stream)
            Me.New(fileStream, Nothing)
        End Sub

        Public Sub New(ByVal fileStream As Stream, ByVal OutputFormat As WaveFormat)
            interopStrm = New InteropStream(fileStream)
            m_reader = WM.CreateSyncReader(WMT_RIGHTS.WMT_RIGHT_NO_DRM)
            Try
                Dim rdr As IWMSyncReader2 = TryCast(m_reader, IWMSyncReader2)
                rdr.OpenStream(interopStrm)
                Init(OutputFormat)
            Catch
                Try
                    m_reader.Close()
                Catch
                End Try
                m_reader = Nothing
                Throw
            End Try
        End Sub

        ''' <summary>
        ''' Create WmaStream with specific format for for uncompressed audio data.
        ''' </summary>
        ''' <param name="FileName">Name of asf file</param>
        ''' <param name="OutputFormat">WaveFormat that define the desired audio data format</param>
        Public Sub New(ByVal FileName As String, ByVal OutputFormat As WaveFormat)
            m_reader = WM.CreateSyncReader(WMT_RIGHTS.WMT_RIGHT_NO_DRM)
            Try
                m_reader.Open(FileName)
                Init(OutputFormat)
            Catch
                Try
                    m_reader.Close()
                Catch
                End Try
                m_reader = Nothing
                Throw
            End Try
        End Sub

        ''' <summary>
        ''' Create WmaStream. The first PCM available for audio outputs will be used as output format.
        ''' Output format can be checked in "WmaStream.Format" property.
        ''' </summary>
        ''' <param name="FileName">Name of asf file</param>
        Public Sub New(ByVal FileName As String)
            Me.New(FileName, Nothing)
        End Sub

        Protected Overrides Sub Finalize()
            Try
                Dispose(False)
            Finally
                MyBase.Finalize()
            End Try
        End Sub

        ''' <summary>
        ''' Give the "WaveLib.WaveFormat" that describes the format of ouput data in each Read operation.
        ''' </summary>
        Public ReadOnly Property Format() As WaveFormat
            Get
                Return New WaveFormat(m_outputFormat.SampleRate, m_outputFormat.BitsPerSample, m_outputFormat.Channels)
            End Get
        End Property

        ''' <summary>
        ''' IWMProfile of the input ASF file.
        ''' </summary>
        Public ReadOnly Property Profile() As IWMProfile
            Get
                Return DirectCast(m_reader, IWMProfile)
            End Get
        End Property

        ''' <summary>
        ''' IWMHeaderInfo related to the input ASF file.
        ''' </summary>
        Public ReadOnly Property HeaderInfo() As IWMHeaderInfo
            Get
                Return DirectCast(m_reader, IWMHeaderInfo)
            End Get
        End Property

        ''' <summary>
        ''' Recomended size of buffer in each "WmaStream.Read" operation
        ''' </summary>
        Public ReadOnly Property SampleSize() As Integer
            Get
                Return CInt(m_sampleSize)
            End Get
        End Property

        ''' <summary>
        ''' If Seek if allowed every seek operation must be to a value multiple of SeekAlign
        ''' </summary>
        Public ReadOnly Property SeekAlign() As Long
            Get
                Return Math.Max(SampleTime2BytePosition(1), CLng(m_outputFormat.BlockAlign))
            End Get
        End Property

        ''' <summary>
        ''' Convert a time value in 100 nanosecond unit to a byte position 
        ''' of byte array containing the PCM data. "WmaStream.BytePosition2SampleTime"
        ''' </summary>
        ''' <param name="SampleTime">Sample time in 100 nanosecond units</param>
        ''' <returns>Byte position</returns>
        Protected Function SampleTime2BytePosition(ByVal SampleTime As ULong) As Long
            Dim res As ULong = SampleTime * CULng(m_outputFormat.AverageBytesPerSecond) / 10000000
            'Align to sample position
            res -= (res Mod CULng(m_outputFormat.BlockAlign))
            Return CLng(res)
        End Function

        ''' <summary>
        ''' Returns the sample time in 100 nanosecond units corresponding to
        ''' byte position in a byte array of PCM data. "WmaStream.SampleTime2BytePosition"
        ''' </summary>
        ''' <param name="pos">Byte position</param>
        ''' <returns>Sample time in 100 nanosecond units</returns>
        Protected Function BytePosition2SampleTime(ByVal pos As Long) As ULong
            'Align to sample position
            pos -= (pos Mod CLng(m_outputFormat.BlockAlign))
            Return CULng(pos) * 10000000 / CULng(m_outputFormat.AverageBytesPerSecond)
        End Function

        ''' <summary>
        ''' Index that give the string representation of the Metadata attribute whose
        ''' name is the string index. If the Metadata is not present returns <code>null</code>. 
        ''' This only return the file level Metadata info, to read stream level metadata use "WmaStream.HeaderInfo"
        ''' </summary>
        ''' <example>
        ''' <code>
        ''' using (WmaStream str = new WmaStream("somefile.asf") )
        ''' {
        '''   string Title = str[WM.g_wszWMTitle];
        '''   if ( Title != null )
        '''   {
        '''     Console.WriteLine("Title: {0}", Title);
        '''   }
        ''' }
        ''' </code>
        ''' </example>
        <System.Runtime.CompilerServices.IndexerName("Attributes")>
        Default Public ReadOnly Property Item(ByVal AttrName As String) As String
            Get
                Dim head As New WMHeaderInfo(HeaderInfo)
                Try
                    Return head(AttrName).Value.ToString()
                Catch e As COMException
                    If e.ErrorCode = WM.ASF_E_NOTFOUND Then
                        Return Nothing
                    Else
                        Throw (e)
                    End If
                End Try
            End Get
        End Property

#Region "Private methods to interact with the WMF"

        Private Sub Init(ByVal OutputFormat As WaveFormat)
            m_outputNumber = GetAudioOutputNumber(m_reader)
            If m_outputNumber = InvalidOuput Then
                Throw New ArgumentException("An audio stream was not found")
            End If
            Dim FormatIndexes As Integer() = GetPCMOutputNumbers(m_reader, CUInt(m_outputNumber))
            If FormatIndexes.Length = 0 Then
                Throw New ArgumentException("An audio stream was not found")
            End If
            If OutputFormat IsNot Nothing Then
                m_outputFormatNumber = -1
                For i As Integer = 0 To FormatIndexes.Length - 1
                    Dim fmt As WaveFormat = GetOutputFormat(m_reader, CUInt(m_outputNumber), CUInt(FormatIndexes(i)))
                    ' (fmt.wFormatTag == OutputFormat.wFormatTag) &&
                    If (fmt.AverageBytesPerSecond = OutputFormat.AverageBytesPerSecond) AndAlso (fmt.BlockAlign = OutputFormat.BlockAlign) AndAlso (fmt.Channels = OutputFormat.Channels) AndAlso (fmt.SampleRate = OutputFormat.SampleRate) AndAlso (fmt.BitsPerSample = OutputFormat.BitsPerSample) Then
                        m_outputFormatNumber = FormatIndexes(i)
                        m_outputFormat = fmt
                        Exit For
                    End If
                Next
                If m_outputFormatNumber < 0 Then
                    Throw New ArgumentException("No PCM output found")
                End If
            Else
                m_outputFormatNumber = FormatIndexes(0)
                m_outputFormat = GetOutputFormat(m_reader, CUInt(m_outputNumber), CUInt(FormatIndexes(0)))
            End If
            Dim OutputCtns As UInteger = 0
            m_reader.GetOutputCount(OutputCtns)
            Dim StreamNumbers As UShort() = New UShort(OutputCtns - 1) {}
            Dim StreamSelections As WMT_STREAM_SELECTION() = New WMT_STREAM_SELECTION(OutputCtns - 1) {}
            For i As UInteger = 0 To OutputCtns - 1
                m_reader.GetStreamNumberForOutput(i, StreamNumbers(i))
                If i = m_outputNumber Then
                    StreamSelections(i) = WMT_STREAM_SELECTION.WMT_ON
                    m_outputStream = StreamNumbers(i)
                    m_reader.SetReadStreamSamples(m_outputStream, False)
                Else
                    StreamSelections(i) = WMT_STREAM_SELECTION.WMT_OFF
                End If
            Next
            m_reader.SetStreamsSelected(CUShort(OutputCtns), StreamNumbers, StreamSelections)
            Dim Props As IWMOutputMediaProps = Nothing
            m_reader.GetOutputFormat(CUInt(m_outputNumber), CUInt(m_outputFormatNumber), Props)
            m_reader.SetOutputProps(CUInt(m_outputNumber), Props)

            Dim size As Integer = 0
            Props.GetMediaType(IntPtr.Zero, size)
            Dim buffer As IntPtr = Marshal.AllocCoTaskMem(size)
            Try
                Dim mt As WM_MEDIA_TYPE
                Props.GetMediaType(buffer, size)
                mt = DirectCast(Marshal.PtrToStructure(buffer, GetType(WM_MEDIA_TYPE)), WM_MEDIA_TYPE)
                m_sampleSize = mt.lSampleSize
            Finally
                Marshal.FreeCoTaskMem(buffer)
                Props = Nothing
            End Try

            m_seekable = False
            m_length = -1
            Dim head As New WMHeaderInfo(HeaderInfo)
            Try
                m_seekable = CBool(head(WM.g_wszWMSeekable))
                ' Yuval Naveh
                Dim nanoDuration As ULong = CULng(head(WM.g_wszWMDuration))
                m_duration = New TimeSpan(CLng(nanoDuration))
                m_length = SampleTime2BytePosition(nanoDuration)
            Catch e As COMException
                If e.ErrorCode <> WM.ASF_E_NOTFOUND Then
                    Throw (e)
                End If
            End Try

        End Sub

        Private Shared Function GetAudioOutputNumber(ByVal Reader As IWMSyncReader) As UInteger
            Dim res As UInteger = InvalidOuput
            Dim OutCounts As UInteger = 0
            Reader.GetOutputCount(OutCounts)
            For i As UInteger = 0 To OutCounts - 1
                Dim Props As IWMOutputMediaProps = Nothing
                Reader.GetOutputProps(i, Props)
                Dim mt As Guid
                Props.[GetType](mt)
                If mt = MediaTypes.WMMEDIATYPE_Audio Then
                    res = i
                    Exit For
                End If
            Next
            Return res
        End Function

        Protected Const WAVE_FORMAT_EX_SIZE As UInteger = 18

        Private Shared Function GetPCMOutputNumbers(ByVal Reader As IWMSyncReader, ByVal OutputNumber As UInteger) As Integer()
            Dim result = New List(Of Integer)()
            Dim FormatCount As UInteger = 0
            Reader.GetOutputFormatCount(OutputNumber, FormatCount)
            Dim BufferSize As Integer = Marshal.SizeOf(GetType(WM_MEDIA_TYPE)) + Marshal.SizeOf(GetType(WaveFormat))
            Dim buffer As IntPtr = Marshal.AllocCoTaskMem(BufferSize)
            Try
                For i As Integer = 0 To FormatCount - 1
                    Dim Props As IWMOutputMediaProps = Nothing
                    Dim size As Integer = 0
                    Dim mt As WM_MEDIA_TYPE
                    Reader.GetOutputFormat(OutputNumber, CUInt(i), Props)
                    Props.GetMediaType(IntPtr.Zero, size)
                    If size > BufferSize Then
                        BufferSize = size
                        Marshal.FreeCoTaskMem(buffer)
                        buffer = Marshal.AllocCoTaskMem(BufferSize)
                    End If
                    Props.GetMediaType(buffer, size)
                    mt = DirectCast(Marshal.PtrToStructure(buffer, GetType(WM_MEDIA_TYPE)), WM_MEDIA_TYPE)
                    If (mt.majortype = MediaTypes.WMMEDIATYPE_Audio) AndAlso (mt.subtype = MediaTypes.WMMEDIASUBTYPE_PCM) AndAlso (mt.formattype = MediaTypes.WMFORMAT_WaveFormatEx) AndAlso (mt.cbFormat >= WAVE_FORMAT_EX_SIZE) Then
                        result.Add(i)
                    End If
                Next
            Finally
                Marshal.FreeCoTaskMem(buffer)
            End Try
            Return result.ToArray()
        End Function

        Private Shared Function GetOutputFormat(ByVal reader As IWMSyncReader, ByVal outputNumber As UInteger, ByVal formatNumber As UInteger) As WaveFormat
            Dim Props As IWMOutputMediaProps = Nothing
            Dim size As Integer = 0
            Dim fmt As WaveFormat = Nothing
            reader.GetOutputFormat(outputNumber, formatNumber, Props)
            Props.GetMediaType(IntPtr.Zero, size)
            Dim buffer As IntPtr = Marshal.AllocCoTaskMem(Math.Max(size, Marshal.SizeOf(GetType(WM_MEDIA_TYPE)) + Marshal.SizeOf(GetType(WaveFormat))))
            Try
                Props.GetMediaType(buffer, size)
                Dim mt = DirectCast(Marshal.PtrToStructure(buffer, GetType(WM_MEDIA_TYPE)), WM_MEDIA_TYPE)
                If (mt.majortype = MediaTypes.WMMEDIATYPE_Audio) AndAlso (mt.subtype = MediaTypes.WMMEDIASUBTYPE_PCM) AndAlso (mt.formattype = MediaTypes.WMFORMAT_WaveFormatEx) AndAlso (mt.cbFormat >= WAVE_FORMAT_EX_SIZE) Then
                    fmt = New WaveFormat(44100, 16, 2)
                    Marshal.PtrToStructure(mt.pbFormat, fmt)
                Else
                    Throw New ArgumentException(String.Format("The format {0} of the output {1} is not a valid PCM format", formatNumber, outputNumber))
                End If
            Finally
                Marshal.FreeCoTaskMem(buffer)
            End Try
            Return fmt
        End Function
#End Region

#Region "Overrided Stream methods"
        Public Overrides Sub Close()
            If m_reader IsNot Nothing Then
                m_reader.Close()
                m_reader = Nothing
            End If
            MyBase.Close()
        End Sub

        Private m_BufferReader As NSSBuffer = Nothing

        Public Overrides Function Read(ByVal buffer As Byte(), ByVal offset As Integer, ByVal count As Integer) As Integer
            If m_reader IsNot Nothing Then
                Dim read__1 As Integer = 0
                If (m_length > 0) AndAlso ((m_length - m_position) < count) Then
                    count = CInt(m_length - m_position)
                End If
                If m_BufferReader IsNot Nothing Then
                    While (m_BufferReader.Position < m_BufferReader.Length) AndAlso (read__1 < count)
                        read__1 += m_BufferReader.Read(buffer, offset, count)
                    End While
                End If
                While read__1 < count
                    Dim sample As INSSBuffer = Nothing
                    Dim SampleTime As ULong = 0
                    Dim Duration As ULong = 0
                    Dim Flags As UInteger = 0
                    Try
                        m_reader.GetNextSample(m_outputStream, sample, SampleTime, Duration, Flags, m_outputNumber,
                         m_outputStream)
                    Catch e As COMException
                        If e.ErrorCode = WM.NS_E_NO_MORE_SAMPLES Then
                            'No more samples
                            If m_outputFormat.BitsPerSample = 8 Then
                                While read__1 < count
                                    buffer(offset + read__1) = &H80
                                    read__1 += 1
                                End While
                            Else
                                Array.Clear(buffer, offset + read__1, count - read__1)
                                read__1 = count
                            End If
                            Exit Try
                        Else
                            Throw (e)
                        End If
                    End Try
                    If sample IsNot Nothing Then
                        m_BufferReader = New NSSBuffer(sample)
                        read__1 += m_BufferReader.Read(buffer, offset + read__1, count - read__1)
                    End If
                End While
                If (m_BufferReader IsNot Nothing) AndAlso (m_BufferReader.Position >= m_BufferReader.Length) Then
                    m_BufferReader = Nothing
                End If
                m_position += read__1
                Return read__1
            Else
                Throw New ObjectDisposedException(Me.ToString())
            End If
        End Function

        Public Overrides Sub Write(ByVal buffer As Byte(), ByVal offset As Integer, ByVal count As Integer)
            Throw New NotSupportedException()
        End Sub

        Public Overrides Function Seek(ByVal offset As Long, ByVal origin As SeekOrigin) As Long
            If CanSeek Then
                Select Case origin
                    Case SeekOrigin.Current
                        offset += m_position
                        Exit Select
                    Case SeekOrigin.[End]
                        offset += m_length
                        Exit Select
                End Select
                If offset = m_position Then
                    ' :-)
                    Return m_position
                End If
                If (offset < 0) OrElse (offset > m_length) Then
                    Throw New ArgumentException("Offset out of range", "offset")
                End If
                If (offset Mod SeekAlign) > 0 Then
                    Throw New ArgumentException(String.Format("Offset must be aligned by a value of SeekAlign ({0})", SeekAlign), "offset")
                End If
                Dim SampleTime As ULong = BytePosition2SampleTime(offset)
                m_reader.SetRange(SampleTime, 0)
                m_position = offset
                m_BufferReader = Nothing
                Return offset
            Else
                Throw New NotSupportedException()
            End If
        End Function

        Public Overrides Sub Flush()
        End Sub

        Public Overrides Sub SetLength(ByVal value As Long)
            Throw New NotSupportedException()
        End Sub

        Public Overrides ReadOnly Property CanRead() As Boolean
            Get
                If m_reader IsNot Nothing Then
                    Return True
                Else
                    Throw New ObjectDisposedException(Me.ToString())
                End If
            End Get
        End Property

        Public Overrides ReadOnly Property CanSeek() As Boolean
            Get
                If m_reader IsNot Nothing Then
                    Return m_seekable AndAlso (m_length > 0)
                Else
                    Throw New ObjectDisposedException(Me.ToString())
                End If
            End Get
        End Property

        Public Overrides ReadOnly Property CanWrite() As Boolean
            Get
                Return False
            End Get
        End Property

        Public ReadOnly Property Duration() As TimeSpan
            Get
                Return m_duration
            End Get
        End Property

        Public Overrides ReadOnly Property Length() As Long
            Get
                If m_reader IsNot Nothing Then
                    If CanSeek Then
                        Return m_length
                    Else
                        Throw New NotSupportedException()
                    End If
                Else
                    Throw New ObjectDisposedException(Me.ToString())
                End If
            End Get
        End Property

        Public Overrides Property Position() As Long
            Get
                If m_reader IsNot Nothing Then
                    Return m_position
                Else
                    Throw New ObjectDisposedException(Me.ToString())
                End If
            End Get
            Set(ByVal value As Long)
                Seek(value, SeekOrigin.Begin)
            End Set
        End Property
#End Region

        Protected Overrides Sub Dispose(ByVal disposing As Boolean)
            If disposing Then
                If m_reader IsNot Nothing Then
                    m_reader.Close()
                    m_reader = Nothing
                End If
            End If
        End Sub

        Private m_reader As IWMSyncReader = Nothing
        Private m_outputNumber As UInteger
        Private m_outputStream As UShort
        Private m_outputFormatNumber As Integer
        Private m_position As Long = 0
        Private m_length As Long = -1
        Private m_seekable As Boolean = False
        Private m_sampleSize As UInteger = 0
        Private m_outputFormat As WaveFormat = Nothing
        Private Const InvalidOuput As UInteger = &HFFFFFFFFUI

        Private m_duration As TimeSpan
    End Class

End Class