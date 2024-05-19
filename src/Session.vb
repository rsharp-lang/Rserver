Imports System.Collections.Specialized
Imports System.IO
Imports System.Text
Imports Flute.Http.Configurations
Imports Flute.Http.Core.HttpStream
Imports Flute.SessionManager
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Net.HTTP
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.Rsharp.Runtime

''' <summary>
''' session data access
''' </summary>
<Package("session")>
Module Session

    Dim ssid As String
    Dim config As Configuration
    Dim file As SessionFile

    <ExportAPI("load")>
    Public Function load(Optional env As Environment = Nothing) As String
        Console.WriteLine("open std in")
        Dim std_in As New StreamReader(Console.OpenStandardInput)
        Dim boundary As String = std_in.ReadLine
        Console.WriteLine(boundary)
        Dim form As String = std_in.ReadLine

        Console.WriteLine(form)
        Dim buf As New MemoryStream(form.Base64RawBytes)
        Dim reader As New PostReader.ContentOutput With {
            .files = New Dictionary(Of String, List(Of HttpPostedFile)),
            .form = New NameValueCollection
        }

        Call std_in.Close()
        Call PostReader.loadMultiPart(boundary, buf, reader, Encoding.UTF8)

        Dim cookie_str As String = reader.form("cookies")
        Dim config_str As String = reader.form("configs")
        Dim cookies As Dictionary(Of String, String) = cookie_str.LoadJSON(Of Dictionary(Of String, String))
        Dim ssid As String = cookies.TryGetValue("session_id")

        Session.ssid = ssid
        Session.config = config_str.LoadJSON(Of Configuration)
        Session.file = Flute.SessionManager.Open(ssid, config)

        Return ssid
    End Function

    <ExportAPI("session_id")>
    Public Function session_id() As String
        Return ssid
    End Function

    <ExportAPI("get_string")>
    Public Function get_string(key As String) As String
        Return file.OpenKeyString(key)
    End Function

    <ExportAPI("get_number")>
    Public Function get_number(key As String) As Double
        Return file.OpenKeyDouble(key)
    End Function

    <ExportAPI("get_integer")>
    Public Function get_integer(key As String) As Integer
        Return file.OpenKeyInteger(key)
    End Function

    <ExportAPI("set_string")>
    Public Function set_string(key As String, value As String) As Object
        Return file.SaveKey(key, value)
    End Function

End Module
