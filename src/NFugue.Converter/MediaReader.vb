Imports System.IO
Imports System.Linq
Imports System.Speech.AudioFormat
Imports System.Speech.Synthesis
Imports AudioSynthesis.Midi
Imports NAudio.Flac
Imports NAudio.Wave
Imports NAudio.Wave.Mp3FileReader
Imports NLayer.NAudioSupport
Imports NVorbis.NAudioSupport
Imports SharpMik

' Original: https://www.codeproject.com/articles/990040/multiwave-a-portable-multi-device-net-audio-player
' Credits to the Original Author: Freefall
Public Class MediaReader
    Inherits WaveStream

    Private Mediafile As String = Nothing

    Private readerStream As WaveStream = Nothing
    Private ReadOnly lockObject As Object = New Object()
    Private SmoothLoop As Boolean = False
    Private ModTitle As String = Nothing
    Private AudioFileStream As FileStream, AudioMemoryStream As MemoryStream
    Private Readable As Boolean = False

    Public Sub New(ByVal fileName As String)
        Mediafile = fileName
        CreateReaderStream(Mediafile)
    End Sub

    Public Sub New(ByVal extension As String, ByVal inputStream As Stream)
        'Mediafile = fileName
        CreateReaderStream(extension, inputStream)
    End Sub

    Private Sub CreateReaderStream(ByVal fileName As String, Optional ByVal useStandardFormat As Boolean = False)
        Try
            If fileName.EndsWith(".mp3", StringComparison.OrdinalIgnoreCase) Then
                Try
                    readerStream = New WmaReader(fileName)
                    SmoothLoop = True
                Catch Mp3Exception1 As Exception
                    Try
                        readerStream = New Mp3FileReader(fileName)
                        SmoothLoop = True
                    Catch Mp3Exception2 As Exception
                        Try
                            readerStream = New Mp3FileReader(fileName, New FrameDecompressorBuilder(Function(waveFormat) New Mp3FrameDecompressor(waveFormat)))
                            SmoothLoop = True
                        Catch Mp3Exception3 As Exception
                            Try
                                If IsWinVistaOrAbove() Then
                                    readerStream = New MediaFoundationReader(fileName)
                                    SmoothLoop = True
                                End If
                            Catch Mp3Exception4 As Exception
                                'Not playable mp3...
                                Exit Sub
                            End Try
                        End Try
                    End Try
                End Try
            ElseIf fileName.EndsWith(".wav", StringComparison.OrdinalIgnoreCase) Then
                readerStream = New WaveFileReader(fileName)
                SmoothLoop = True
            ElseIf fileName.EndsWith(".ogg", StringComparison.OrdinalIgnoreCase) Then
                readerStream = New VorbisFileReader(fileName)
                SmoothLoop = True
            ElseIf fileName.EndsWith(".wma", StringComparison.OrdinalIgnoreCase) Or fileName.EndsWith(".asf", StringComparison.OrdinalIgnoreCase) Or fileName.EndsWith(".mpe", StringComparison.OrdinalIgnoreCase) Or fileName.EndsWith(".wmv", StringComparison.OrdinalIgnoreCase) Or fileName.EndsWith(".avi", StringComparison.OrdinalIgnoreCase) Then
                Try
                    readerStream = New WmaReader(fileName)
                    SmoothLoop = True
                Catch WmaException As Exception
                    'Probably WMVCore.dll missing... use mediafoundation
                    Try
                        If IsWinVistaOrAbove() Then
                            readerStream = New MediaFoundationReader(fileName)
                            SmoothLoop = True
                        End If
                    Catch ex As Exception
                        'Not playable wma...
                        Exit Sub
                    End Try
                End Try
                'ElseIf fileName.EndsWith(".flv", StringComparison.OrdinalIgnoreCase) Then
                '    readerStream = New FLVReader(fileName)
                '    SmoothLoop = True
            ElseIf fileName.EndsWith(".flac", StringComparison.OrdinalIgnoreCase) Then
                readerStream = New FlacReader(fileName)
                SmoothLoop = False
            ElseIf fileName.EndsWith(".aif", StringComparison.OrdinalIgnoreCase) Or fileName.EndsWith(".aiff", StringComparison.OrdinalIgnoreCase) Then
                readerStream = New AiffFileReader(fileName)
                SmoothLoop = False
            ElseIf fileName.EndsWith(".raw", StringComparison.OrdinalIgnoreCase) Then
                Dim Format As WaveFormat
                If useStandardFormat Then
                    Format = New WaveFormat(44100, 16, 2)
                Else
                    Dim SampleRate As Integer = CInt(InputBox("Samplerate? E.g. 44100 (Hz)", , "44100"))
                    Dim BitsPerSample As Integer = CInt(InputBox("Bits per Sample? E.g. 16 (bits)", , "16"))
                    Dim Channels As Integer = CInt(InputBox("Channels? E.g. 2 (for Stereo)", , "2"))
                    Format = New WaveFormat(SampleRate, BitsPerSample, Channels)
                End If
                AudioFileStream = New FileStream(fileName, FileMode.Open)
                readerStream = New RawSourceWaveStream(AudioFileStream, Format)
                SmoothLoop = True
            ElseIf fileName.EndsWith(".mod", StringComparison.OrdinalIgnoreCase) Or fileName.EndsWith(".xm", StringComparison.OrdinalIgnoreCase) Or fileName.EndsWith(".s3m", StringComparison.OrdinalIgnoreCase) _
            Or fileName.EndsWith(".it", StringComparison.OrdinalIgnoreCase) Or fileName.EndsWith(".m15", StringComparison.OrdinalIgnoreCase) Or fileName.EndsWith(".669", StringComparison.OrdinalIgnoreCase) _
            Or fileName.EndsWith(".far", StringComparison.OrdinalIgnoreCase) Or fileName.EndsWith(".ult", StringComparison.OrdinalIgnoreCase) Or fileName.EndsWith(".mtm", StringComparison.OrdinalIgnoreCase) Then
                Dim Reader = New NAudioTrackerStream(fileName)
                ModTitle = CleanModuleTitle(Reader.ModName)
                readerStream = Reader
                SmoothLoop = False
            ElseIf fileName.EndsWith(".mid", StringComparison.OrdinalIgnoreCase) Then
                readerStream = New MidiReader(fileName)
                SmoothLoop = False
            ElseIf fileName.EndsWith(".au", StringComparison.OrdinalIgnoreCase) Or fileName.EndsWith(".snd", StringComparison.OrdinalIgnoreCase) Then
                readerStream = New SunReader(fileName)
                SmoothLoop = True
            ElseIf fileName.EndsWith(".txt", StringComparison.OrdinalIgnoreCase) Then
                Using Synth As New SpeechSynthesizer()
                    AudioMemoryStream = New MemoryStream()
                    Synth.SetOutputToAudioStream(AudioMemoryStream, New SpeechAudioFormatInfo(44100, 16, 2))
                    Synth.Rate = 1
                    Synth.Speak(My.Computer.FileSystem.ReadAllText(fileName))
                    AudioMemoryStream.Position = 0
                End Using
                readerStream = New RawSourceWaveStream(AudioMemoryStream, New WaveFormat())
                SmoothLoop = True
            ElseIf fileName.EndsWith(".m4a", StringComparison.OrdinalIgnoreCase) Or fileName.EndsWith(".m4b", StringComparison.OrdinalIgnoreCase) Or fileName.EndsWith(".mp4", StringComparison.OrdinalIgnoreCase) Then
                If IsWinVistaOrAbove() Then
                    readerStream = New MediaFoundationReader(fileName)
                    SmoothLoop = True
                End If
            Else
                If IsWinVistaOrAbove() Then
                    readerStream = New MediaFoundationReader(fileName)
                    SmoothLoop = True
                End If
            End If
            If readerStream IsNot Nothing Then Readable = True Else Readable = False
        Catch ex As Exception
            Readable = False
            'Sorry, can´t read file...
            Exit Sub
        End Try
    End Sub

    Private Sub CreateReaderStream(ByVal extension As String, ByVal inputStream As Stream, Optional ByVal useStandardFormat As Boolean = False)
        Try
            If extension.EndsWith(".mp3", StringComparison.OrdinalIgnoreCase) Then
                Try
                    readerStream = New WmaReader(inputStream)
                    SmoothLoop = True
                Catch Mp3Exception1 As Exception
                    Try
                        readerStream = New Mp3FileReader(extension)
                        SmoothLoop = True
                    Catch Mp3Exception2 As Exception
                        Try
                            readerStream = New Mp3FileReader(inputStream, New FrameDecompressorBuilder(Function(waveFormat) New Mp3FrameDecompressor(waveFormat)))
                            SmoothLoop = True
                        Catch Mp3Exception3 As Exception
                            'Try
                            '    If IsWinVistaOrAbove() Then
                            '        readerStream = New MediaFoundationReader(inputStream)
                            '        SmoothLoop = True
                            '    End If
                            'Catch Mp3Exception4 As Exception
                            '    'Not playable mp3...
                            '    Exit Sub
                            'End Try
                        End Try
                    End Try
                End Try
            ElseIf extension.EndsWith(".wav", StringComparison.OrdinalIgnoreCase) Then
                readerStream = New WaveFileReader(inputStream)
                SmoothLoop = True
            ElseIf extension.EndsWith(".ogg", StringComparison.OrdinalIgnoreCase) Then
                readerStream = New VorbisFileReader(inputStream)
                SmoothLoop = True
            ElseIf extension.EndsWith(".wma", StringComparison.OrdinalIgnoreCase) Or extension.EndsWith(".asf", StringComparison.OrdinalIgnoreCase) Or extension.EndsWith(".mpe", StringComparison.OrdinalIgnoreCase) Or extension.EndsWith(".wmv", StringComparison.OrdinalIgnoreCase) Or extension.EndsWith(".avi", StringComparison.OrdinalIgnoreCase) Then
                Try
                    readerStream = New WmaReader(inputStream)
                    SmoothLoop = True
                Catch WmaException As Exception
                    'Probably WMVCore.dll missing... use mediafoundation
                    'Try
                    '    If IsWinVistaOrAbove() Then
                    '        readerStream = New MediaFoundationReader(inputStream)
                    '        SmoothLoop = True
                    '    End If
                    'Catch ex As Exception
                    '    'Not playable wma...
                    '    Exit Sub
                    'End Try
                End Try
                'ElseIf fileName.EndsWith(".flv", StringComparison.OrdinalIgnoreCase) Then
                '    readerStream = New FLVReader(fileName)
                '    SmoothLoop = True
            ElseIf extension.EndsWith(".flac", StringComparison.OrdinalIgnoreCase) Then
                readerStream = New FlacReader(inputStream)
                SmoothLoop = False
            ElseIf extension.EndsWith(".aif", StringComparison.OrdinalIgnoreCase) Or extension.EndsWith(".aiff", StringComparison.OrdinalIgnoreCase) Then
                readerStream = New AiffFileReader(inputStream)
                SmoothLoop = False
            ElseIf extension.EndsWith(".raw", StringComparison.OrdinalIgnoreCase) Then
                Dim Format As WaveFormat
                If useStandardFormat Then
                    Format = New WaveFormat(44100, 16, 2)
                Else
                    Dim SampleRate As Integer = CInt(InputBox("Samplerate? E.g. 44100 (Hz)", , "44100"))
                    Dim BitsPerSample As Integer = CInt(InputBox("Bits per Sample? E.g. 16 (bits)", , "16"))
                    Dim Channels As Integer = CInt(InputBox("Channels? E.g. 2 (for Stereo)", , "2"))
                    Format = New WaveFormat(SampleRate, BitsPerSample, Channels)
                End If
                'AudioFileStream = New FileStream(inputStream, FileMode.Open)
                'readerStream = New RawSourceWaveStream(AudioFileStream, Format)
                'SmoothLoop = True
            ElseIf extension.EndsWith(".mod", StringComparison.OrdinalIgnoreCase) Or extension.EndsWith(".xm", StringComparison.OrdinalIgnoreCase) Or extension.EndsWith(".s3m", StringComparison.OrdinalIgnoreCase) _
            Or extension.EndsWith(".it", StringComparison.OrdinalIgnoreCase) Or extension.EndsWith(".m15", StringComparison.OrdinalIgnoreCase) Or extension.EndsWith(".669", StringComparison.OrdinalIgnoreCase) _
            Or extension.EndsWith(".far", StringComparison.OrdinalIgnoreCase) Or extension.EndsWith(".ult", StringComparison.OrdinalIgnoreCase) Or extension.EndsWith(".mtm", StringComparison.OrdinalIgnoreCase) Then
                Dim Reader = New NAudioTrackerStream(extension)
                ModTitle = CleanModuleTitle(Reader.ModName)
                readerStream = Reader
                SmoothLoop = False
            ElseIf extension.EndsWith(".mid", StringComparison.OrdinalIgnoreCase) Then
                readerStream = New MidiReader(inputStream)
                SmoothLoop = False
            ElseIf extension.EndsWith(".au", StringComparison.OrdinalIgnoreCase) Or extension.EndsWith(".snd", StringComparison.OrdinalIgnoreCase) Then
                readerStream = New SunReader(inputStream)
                SmoothLoop = True
            ElseIf extension.EndsWith(".txt", StringComparison.OrdinalIgnoreCase) Then
                'Using Synth As New SpeechSynthesizer()
                '    AudioMemoryStream = New MemoryStream()
                '    Synth.SetOutputToAudioStream(AudioMemoryStream, New SpeechAudioFormatInfo(44100, 16, 2))
                '    Synth.Rate = 1
                '    Synth.Speak(My.Computer.FileSystem.ReadAllText(inputStream))
                '    AudioMemoryStream.Position = 0
                'End Using
                'readerStream = New RawSourceWaveStream(AudioMemoryStream, New WaveFormat())
                'SmoothLoop = True
            ElseIf extension.EndsWith(".m4a", StringComparison.OrdinalIgnoreCase) Or extension.EndsWith(".m4b", StringComparison.OrdinalIgnoreCase) Or extension.EndsWith(".mp4", StringComparison.OrdinalIgnoreCase) Then
                'If IsWinVistaOrAbove() Then
                '    readerStream = New MediaFoundationReader(inputStream)
                '    SmoothLoop = True
                'End If
            Else
                'If IsWinVistaOrAbove() Then
                '    readerStream = New MediaFoundationReader(inputStream)
                '    SmoothLoop = True
                'End If
            End If
            If readerStream IsNot Nothing Then Readable = True Else Readable = False
        Catch ex As Exception
            Readable = False
            'Sorry, can´t read file...
            Exit Sub
        End Try
    End Sub
    Private Function CleanModuleTitle(ByVal Str As String) As String
        Return New String(Str.Where(Function(c) " {}()[]´`*~+€,;:@!?\/$§%&=""^°-_.0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz".Contains(c)).ToArray).Replace("_", " ").Trim
    End Function

    Public Overrides ReadOnly Property CanRead() As Boolean
        Get
            Return Readable
        End Get
    End Property

    Public ReadOnly Property IsModule() As Boolean
        Get
            Return ModTitle <> Nothing
        End Get
    End Property

    Public ReadOnly Property ModuleTitle() As String
        Get
            Return ModTitle
        End Get
    End Property

    Public ReadOnly Property Smooth() As Boolean
        Get
            Return SmoothLoop
        End Get
    End Property

    Public Overrides ReadOnly Property WaveFormat() As WaveFormat
        Get
            Return readerStream.WaveFormat
        End Get
    End Property

    Public Overrides ReadOnly Property Length() As Long
        Get
            Return readerStream.Length
        End Get
    End Property

    Public Overrides Property Position() As Long
        Get
            Return readerStream.Position
        End Get
        Set(ByVal value As Long)
            SyncLock lockObject
                readerStream.Position = value
            End SyncLock
        End Set
    End Property

    Public Overrides Function Read(ByVal buffer As Byte(), ByVal offset As Integer, ByVal count As Integer) As Integer
        SyncLock lockObject
            If readerStream IsNot Nothing Then
                Return readerStream.Read(buffer, offset, count)
            End If
        End SyncLock
    End Function

    Private Function IsWinVistaOrAbove() As Boolean
        If Environment.OSVersion.Version.Major >= 6 And Environment.OSVersion.Version.Minor >= 0 Then
            Return True
        Else
            Return False
        End If
    End Function

    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing Then
            If readerStream IsNot Nothing Then readerStream.Dispose() : readerStream = Nothing
            If AudioFileStream IsNot Nothing Then AudioFileStream.Dispose() : AudioFileStream = Nothing
            If AudioMemoryStream IsNot Nothing Then AudioMemoryStream.Dispose() : AudioMemoryStream = Nothing
        End If
        MyBase.Dispose(disposing)
    End Sub
End Class