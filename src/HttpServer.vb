#Region "Microsoft.VisualBasic::9c9d957d9da6111d1e9444c51404bf3d, win32_desktop\src\Rstudio\Rserver\src\HttpServer.vb"

' Author:
' 
'       asuka (amethyst.asuka@gcmodeller.org)
'       xie (genetics@smrucc.org)
'       xieguigang (xie.guigang@live.com)
' 
' Copyright (c) 2018 GPL3 Licensed
' 
' 
' GNU GENERAL PUBLIC LICENSE (GPL3)
' 
' 
' This program is free software: you can redistribute it and/or modify
' it under the terms of the GNU General Public License as published by
' the Free Software Foundation, either version 3 of the License, or
' (at your option) any later version.
' 
' This program is distributed in the hope that it will be useful,
' but WITHOUT ANY WARRANTY; without even the implied warranty of
' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
' GNU General Public License for more details.
' 
' You should have received a copy of the GNU General Public License
' along with this program. If not, see <http://www.gnu.org/licenses/>.



' /********************************************************************************/

' Summaries:


' Code Statistics:

'   Total Lines: 193
'    Code Lines: 122 (63.21%)
' Comment Lines: 49 (25.39%)
'    - Xml Docs: 97.96%
' 
'   Blank Lines: 22 (11.40%)
'     File Size: 6.86 KB


' Module HttpServer
' 
'     Function: createDriver, customResponseHeader, getHeaders, getHttpRaw, getUrl
'               httpMethod, parseUrl, serve, urlList
' 
'     Sub: httpError, pushDownload
' 
' /********************************************************************************/

#End Region

Imports System.Threading
Imports Flute.Http
Imports Flute.Http.Core
Imports Flute.Http.Core.Message
Imports Flute.Http.FileSystem
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Net.Http
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.Rsharp.Interpreter
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
Imports SMRUCC.Rsharp.Runtime.Components.Interface
Imports SMRUCC.Rsharp.Runtime.Internal.Invokes
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop
Imports SMRUCC.Rsharp.Runtime.Vectorization

<Package("http", Category:=APICategories.UtilityTools)>
Public Module HttpServer

    ''' <summary>
    ''' create a http driver module
    ''' </summary>
    ''' <param name="silent"></param>
    ''' <returns></returns>
    <ExportAPI("http_socket")>
    Public Function createDriver(Optional silent As Boolean = True) As HttpDriver
        Return New HttpDriver(New Configurations.Configuration With {.silent = silent})
    End Function

    <ExportAPI("http_fsdir")>
    <RApiReturn(GetType(WebFileSystemListener))>
    Public Function createFsDir(<RListObjectArgument> wwwroot As Object, Optional env As Environment = Nothing) As Object
        Dim dirs = base.c(wwwroot, env)

        If TypeOf dirs Is Message Then
            Return dirs
        End If

        Dim folders As FileSystem() = CLRVector _
            .asCharacter(dirs) _
            .Select(Function(dir) New FileSystem(dir)) _
            .ToArray

        Return New WebFileSystemListener With {.fs = folders}
    End Function

    <ExportAPI("http_exists")>
    Public Function check_exists(www As WebFileSystemListener, req As HttpRequest) As Boolean
        Return www.CheckResourceFileExists(req)
    End Function

    <ExportAPI("host_file")>
    Public Function hostFile(www As WebFileSystemListener, req As HttpRequest, response As HttpResponse) As Object
        Call www.WebHandler(req, response)
        Return Nothing
    End Function

    ''' <summary>
    ''' get http headers data from the browser request
    ''' </summary>
    ''' <param name="req"></param>
    ''' <returns></returns>
    <ExportAPI("getHeaders")>
    Public Function getHeaders(req As HttpRequest) As list
        Return New list(RType.GetRSharpType(GetType(String))) With {
            .slots = req.HttpHeaders _
                .ToDictionary(Function(h) h.Key,
                              Function(h)
                                  Return CObj(h.Value)
                              End Function)
        }
    End Function

    <ExportAPI("getPostData")>
    Public Function getPostData(req As HttpPOSTRequest, Optional q As String = Nothing) As Object
        If q.StringEmpty(, True) Then
            ' returns all in a list
            Return New list(req.POSTData.Form)
        Else
            ' just returns key related value
            Return CStr(req(q))
        End If
    End Function

    ''' <summary>
    ''' get url data from the browser request 
    ''' </summary>
    ''' <param name="req"></param>
    ''' <returns></returns>
    <ExportAPI("getUrl")>
    Public Function getUrl(req As HttpRequest) As list
        Return urlList(req.URL)
    End Function

    Private Function urlList(url As URL) As list
        Dim queryData As New list(RType.GetRSharpType(GetType(String))) With {
            .slots = url.query _
                .ToDictionary(Function(q) q.Key,
                              Function(q)
                                  Return CObj(q.Value)
                              End Function)
        }

        Return New list With {
            .slots = New Dictionary(Of String, Object) From {
                {"url", url.ToString},
                {"path", url.path},
                {"hostName", url.hostName},
                {"hash", url.hashcode},
                {"query", queryData},
                {"port", url.port},
                {"protocol", url.protocol}
            }
        }
    End Function

    <ExportAPI("parseUrl")>
    Public Function parseUrl(url As String) As list
        Return urlList(New URL(url))
    End Function

    ''' <summary>
    ''' get the raw http request header from the browser request
    ''' </summary>
    ''' <param name="req"></param>
    ''' <returns></returns>
    <ExportAPI("getHttpRaw")>
    Public Function getHttpRaw(req As HttpRequest) As String
        Return req.HttpRequest.raw
    End Function

    ''' <summary>
    ''' add custom http response headers
    ''' </summary>
    ''' <param name="driver"></param>
    ''' <param name="headers"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("headers")>
    Public Function customResponseHeader(driver As HttpDriver,
                                         <RListObjectArgument>
                                         headers As list,
                                         Optional env As Environment = Nothing) As HttpDriver

        For Each header As KeyValuePair(Of String, String) In headers.AsGeneric(Of String)(env)
            Call driver.AddResponseHeader(header.Key, header.Value)
        Next

        Return driver
    End Function

    ''' <summary>
    ''' list to the specific tcp port and run the R# http web server
    ''' </summary>
    ''' <param name="driver"></param>
    ''' <param name="port%"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("listen")>
    Public Function serve(driver As HttpDriver,
                          Optional port% = -1,
                          Optional env As Environment = Nothing) As Integer

        Dim httpPort As Integer = If(port <= 0, Rnd() * 30000, port)
        Dim socket As HttpSocket = driver.GetSocket(httpPort)
        Dim localUrl$ = $"http://localhost:{httpPort}/"

        If env.globalEnvironment.debugMode Then
            Call New Thread(
                Sub()
                    Try
                        ' ignore error for debug on linux platform
                        Call Thread.Sleep(3000)
                        Call Process.Start(New ProcessStartInfo With {.FileName = localUrl, .UseShellExecute = True})
                    Catch ex As Exception
                    End Try
                End Sub) _
                         _
            .Start()
        End If

        Return socket.Run()
    End Function

    ''' <summary>
    ''' write http error code and send error response to browser
    ''' </summary>
    ''' <param name="write"></param>
    ''' <param name="code"></param>
    ''' <param name="message"></param>
    <ExportAPI("httpError")>
    Public Sub httpError(write As HttpResponse, code As Integer, message As String)
        Call write.WriteError(code, message)
    End Sub

    ''' <summary>
    ''' set http method handler to the R# web server
    ''' </summary>
    ''' <param name="driver"></param>
    ''' <param name="method"></param>
    ''' <param name="process"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("httpMethod")>
    Public Function httpMethod(driver As HttpDriver,
                               method As String,
                               process As RFunction,
                               Optional accessAny As Boolean = False,
                               Optional env As Environment = Nothing) As HttpDriver

        Call driver.HttpMethod(
            method:=method.ToUpper,
            handler:=Sub(req, response)
                         If accessAny Then
                             response.AccessControlAllowOrigin = "*"
                         End If

                         Dim result = process.Invoke({req, response}, env)

                         If Program.isException(result) Then
                             ' return http 500
                             Call response.WriteError(500, DirectCast(result, Message).ToErrorText)
                             Call response.Flush()
                         End If
                     End Sub)

        Return driver
    End Function

    <ExportAPI("pushDownload")>
    Public Sub pushDownload(response As HttpResponse, filename As String)
        If Not filename.FileExists Then
            Call response.WriteError(404, "not found!")
        Else
            Call response.SendFile(filename)
        End If
    End Sub
End Module
