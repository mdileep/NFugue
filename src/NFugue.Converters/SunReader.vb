Imports System.IO
Imports System.Linq
Imports NAudio.Wave


''' <summary>This class supports the reading of AU and SND files,
''' providing a repositionable WaveStream that returns the raw data
''' contained in the file. Based on wavefilereader class.
''' Using: https://de.wikipedia.org/wiki/Au_%28Dateiformat%29
''' Better source: https://en.wikipedia.org/wiki/Au_file_format
''' </summary>
Friend Class SunReader
    Inherits WaveStream

    Private ReadOnly m_waveFormat As WaveFormat
    Private ReadOnly ownInput As Boolean
    Private ReadOnly dataPosition As Long
    Private ReadOnly dataChunkLength As Long
    Private ReadOnly lockObject As New Object()
    Private ReadOnly Encoding As Integer
    Private ReadOnly InformationBlock As String
    Private waveStream As Stream

    Public Sub New(ByVal SunAudioFile As String)
        Me.New(File.OpenRead(SunAudioFile))
        ownInput = True
    End Sub

    Public Sub New(ByVal SunAudioStream As Stream)
        Me.waveStream = SunAudioStream
        ReadSunHeader(waveStream, m_waveFormat, dataPosition, dataChunkLength, Encoding, InformationBlock)
        Position = 0
    End Sub

    Private Sub ReadSunHeader(ByRef str As Stream, ByRef m_waveFormat As WaveFormat, ByRef dataPosition As Long, ByRef dataChunkLength As Long, ByRef Encoding As Integer, ByRef Infoblock As String)
        Try
            Dim reader As New BinaryReader(str)
            If New String(reader.ReadChars(4)) <> ".snd" Then
                Throw New FormatException("Not a Sun Audio file")
            End If
            dataPosition = BitConverter.ToInt32(reader.ReadBytes(4).Reverse.ToArray, 0)
            If dataPosition < 24 Then
                Throw New FormatException("Invalid data position")
            End If
            dataChunkLength = BitConverter.ToInt32(reader.ReadBytes(4).Reverse.ToArray, 0)
            If Not dataChunkLength > 0 Then
                'Chunk length unknown. Use default value.
                dataChunkLength = &HFFFFFFFF
            End If
            Encoding = BitConverter.ToInt32(reader.ReadBytes(4).Reverse.ToArray, 0)
            'Possible encodings:
            '1 = 8-bit G.711 µ-law
            '2 = 8-bit linear PCM
            '3 = 16-bit linear PCM
            '4 = 24-bit linear PCM
            '5 = 32-bit linear PCM
            '6 = 32-bit IEEE floating point
            '7 = 64-bit IEEE floating point
            '8 = Fragmented sample data
            '9 = DSP program
            '10 = 8-bit fixed point
            '11 = 16-bit fixed point
            '12 = 24-bit fixed point
            '13 = 32-bit fixed point
            '18 = 16-bit linear with emphasis
            '19 = 16-bit linear compressed
            '20 = 16-bit linear with emphasis and compression
            '21 = Music kit DSP commands
            '23 = 4-bit compressed using the ITU-T G.721 ADPCM voice data encoding scheme
            '24 = ITU-T G.722 SB-ADPCM
            '25 = ITU-T G.723 3-bit ADPCM
            '26 = ITU-T G.723 5-bit ADPCM
            '27 = 8-bit G.711 A-law
            Dim SampleRate As Integer = BitConverter.ToInt32(reader.ReadBytes(4).Reverse.ToArray, 0)
            Dim Channels As Integer = BitConverter.ToInt32(reader.ReadBytes(4).Reverse.ToArray, 0)
            Select Case Encoding
                Case 1 : m_waveFormat = WaveFormat.CreateCustomFormat(WaveFormatEncoding.MuLaw, SampleRate, Channels, SampleRate * Channels * 1, Channels * 1, 8)
                Case 2 : m_waveFormat = WaveFormat.CreateCustomFormat(WaveFormatEncoding.Pcm, SampleRate, Channels, SampleRate * Channels * 1, Channels * 1, 8)
                Case 3 : m_waveFormat = WaveFormat.CreateCustomFormat(WaveFormatEncoding.Pcm, SampleRate, Channels, SampleRate * Channels * 2, Channels * 2, 16)
                Case 4 : m_waveFormat = WaveFormat.CreateCustomFormat(WaveFormatEncoding.Pcm, SampleRate, Channels, SampleRate * Channels * 3, Channels * 3, 24)
                Case 5 : m_waveFormat = WaveFormat.CreateCustomFormat(WaveFormatEncoding.Pcm, SampleRate, Channels, SampleRate * Channels * 4, Channels * 4, 32)
                Case 6 : m_waveFormat = WaveFormat.CreateCustomFormat(WaveFormatEncoding.IeeeFloat, SampleRate, Channels, SampleRate * Channels * 4, Channels * 4, 32)
                Case 7 : m_waveFormat = WaveFormat.CreateCustomFormat(WaveFormatEncoding.IeeeFloat, SampleRate, Channels, SampleRate * Channels * 8, Channels * 8, 64)
                Case 27 : m_waveFormat = WaveFormat.CreateALawFormat(SampleRate, Channels)
                Case 23 To 26, 28 To 36 : m_waveFormat = New AdpcmWaveFormat(SampleRate, Channels)
                Case Else : Throw New FormatException("Unknown or unsupported encoding")
            End Select
            'Read info block when existing.
            If waveStream.Position <> dataPosition Then
                Infoblock = New String(reader.ReadChars(dataPosition - waveStream.Position))
            End If
        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    Public ReadOnly Property Infoblock() As String
        Get
            Return InformationBlock
        End Get
    End Property

    Public Overrides ReadOnly Property WaveFormat() As WaveFormat
        Get
            Return m_waveFormat
        End Get
    End Property

    Public Overrides ReadOnly Property Length() As Long
        Get
            Return dataChunkLength
        End Get
    End Property

    Public Overrides Property Position() As Long
        Get
            Return waveStream.Position - dataPosition
        End Get
        Set(ByVal value As Long)
            SyncLock lockObject
                value = Math.Min(value, Length)
                'make sure we don't get out of sync
                value -= (value Mod WaveFormat.BlockAlign)
                waveStream.Position = value + dataPosition
            End SyncLock
        End Set
    End Property

    Public Overrides Function Read(ByVal array As Byte(), ByVal offset As Integer, ByVal count As Integer) As Integer
        If count Mod m_waveFormat.BlockAlign <> 0 Then
            Throw New ArgumentException(String.Format("Must read complete blocks: requested {0}, block align is {1}", count, Me.WaveFormat.BlockAlign))
        End If
        SyncLock lockObject
            ' sometimes there is more junk at the end of the file past the data chunk
            If Position + count > dataChunkLength Then
                count = CInt(dataChunkLength - Position)
            End If
            'Sun file is always big endian. This block can probably be improved by using bitshifting.
            If (m_waveFormat.BitsPerSample / 8) > 1 Then
                For i = 0 To count - 1 Step (m_waveFormat.BitsPerSample / 8)
                    For j = (m_waveFormat.BitsPerSample / 8) - 1 To 0 Step -1
                        array(j + i) = waveStream.ReadByte()
                    Next
                Next
            Else
                waveStream.Read(array, offset, count)
            End If
            Return count
        End SyncLock
    End Function

    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing Then
            ' Release managed resources.
            If waveStream IsNot Nothing Then
                ' only dispose our source if we created it
                If ownInput Then
                    waveStream.Close()
                End If
                waveStream = Nothing
            End If
        Else
            System.Diagnostics.Debug.Assert(False, "SunReader was not disposed")
        End If
        ' Release unmanaged resources.
        ' Set large fields to null.
        ' Call Dispose on your base class.
        MyBase.Dispose(disposing)
    End Sub

End Class