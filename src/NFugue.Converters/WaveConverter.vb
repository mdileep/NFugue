Imports System.IO
Imports NAudio.Wave
Imports NAudio.Wave.SampleProviders

' Original: https://www.codeproject.com/articles/990040/multiwave-a-portable-multi-device-net-audio-player
' Credits to the Original Author: Freefall
Public Class WaveConverter
    Public Shared Sub Convert(TBInput As String, TBOutput As String)

        If TBInput = Nothing Or TBOutput = Nothing Then
            Throw New Exception("Path undefined, aborted.")
            Exit Sub
        End If

        Dim waveReader As IWaveProvider = Nothing
        Using Reader = New MediaReader(TBInput)
            If Not Reader.CanRead Then Throw New FormatException("Not readable file.")
            waveReader = Reader
            'Resample and in any case change bit depth to 16bit.
            If waveReader.WaveFormat.SampleRate = 44100 Then
                waveReader = New SampleToWaveProvider16(New WdlResamplingSampleProvider(waveReader.ToSampleProvider, 44100))
            Else
                waveReader = New SampleToWaveProvider16(waveReader.ToSampleProvider)
            End If

            'Go PCM in any case.
            If waveReader.WaveFormat.Encoding = WaveFormatEncoding.IeeeFloat Then
                waveReader = New WaveFloatTo16Provider(waveReader)
            End If

            Dim buffer As Byte() = New Byte(4095) {}
            Dim bytesRead As Integer = 0

            Using Writer As New WaveFileWriter(TBOutput, waveReader.WaveFormat)
                Do
                    bytesRead = waveReader.Read(buffer, 0, buffer.Length)
                    Writer.Write(buffer, 0, bytesRead)

                Loop While bytesRead > 0
            End Using
        End Using

    End Sub

    Public Shared Sub Convert(ByVal extension As String, inputStream As Stream, outputStream As Stream)

        'If inputStream = Nothing Or outputStream = Nothing Then
        '    Throw New Exception("Path undefined, aborted.")
        '    Exit Sub
        'End If

        inputStream.Seek(0, SeekOrigin.Begin)

        Dim waveReader As IWaveProvider = Nothing
        Using Reader = New MediaReader(extension, inputStream)
            If Not Reader.CanRead Then Throw New FormatException("Not readable file.")
            waveReader = Reader
            'Resample and in any case change bit depth to 16bit.
            If waveReader.WaveFormat.SampleRate = 44100 Then
                waveReader = New SampleToWaveProvider16(New WdlResamplingSampleProvider(waveReader.ToSampleProvider, 44100))
            Else
                waveReader = New SampleToWaveProvider16(waveReader.ToSampleProvider)
            End If

            'Go PCM in any case.
            If waveReader.WaveFormat.Encoding = WaveFormatEncoding.IeeeFloat Then
                waveReader = New WaveFloatTo16Provider(waveReader)
            End If

            Dim buffer As Byte() = New Byte(4095) {}
            Dim bytesRead As Integer = 0

            'Using Writer As New WaveFileWriter(outputStream, waveReader.WaveFormat)
            Dim Writer As New WaveFileWriter(outputStream, waveReader.WaveFormat)
            Do
                bytesRead = waveReader.Read(buffer, 0, buffer.Length)
                Writer.Write(buffer, 0, bytesRead)

            Loop While bytesRead > 0
            'End Using
            Writer.Flush()
            'Writer.Dispose()
        End Using
    End Sub
End Class
