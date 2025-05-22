Imports System.IO
Imports System.Runtime.CompilerServices
Imports Flute.Http.Core
Imports Flute.Http.Core.Message
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Net.Http
Imports Rserver
Imports SMRUCC.Rsharp.Interpreter.ExecuteEngine
Imports SMRUCC.Rsharp.Interpreter.ExecuteEngine.ExpressionSymbols.Closure
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
Imports SMRUCC.Rsharp.Runtime.Internal.[Object]
Imports SMRUCC.Rsharp.Runtime.Interop
Imports SMRUCC.Rsharp.Runtime.Serialize
Imports SMRUCC.Rsharp.Runtime.Vectorization

''' <summary>
''' http url router
''' </summary>
''' <remarks>
''' redirect of the http request to the specific runtime function
''' </remarks>
Public Class Router

    ReadOnly gets As New Dictionary(Of String, DeclareNewFunction)
    ReadOnly posts As New Dictionary(Of String, DeclareNewFunction)

    ''' <summary>
    ''' Check of the url is existed inside current router?
    ''' </summary>
    ''' <param name="url"></param>
    ''' <returns></returns>
    ''' 
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function CheckUrl(url As String) As Boolean
        Return gets.ContainsKey(url) OrElse posts.ContainsKey(url)
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Sub SetUrl(url As String, method As String, handler As DeclareNewFunction)
        Select Case Strings.LCase(method)
            Case "get" : gets(url) = handler
            Case "post" : posts(url) = handler
            Case Else
                Throw New NotImplementedException(method)
        End Select
    End Sub

    Public Function HandleRequest(req As HttpRequest, response As HttpResponse, env As Environment) As Object
        Dim url As URL = req.URL
        Dim func = If(TypeOf req Is HttpPOSTRequest, posts, gets).TryGetValue(url.path)
        Dim writeLine = env.WriteLineHandler

        Call writeLine($" -> [{url.path}]")

        If func Is Nothing Then
            ' 404
            Return 404
        End If

        Dim args As Dictionary(Of String, Object) = req.GetArguments

        If TypeOf req Is HttpPOSTRequest AndAlso func.getArguments.Any(Function(a) a.Name = "upload") Then
            Dim files = DirectCast(req, HttpPOSTRequest).POSTData.files
            Dim file = files.TryGetValue("File")
            Dim firstFile = If(files.IsNullOrEmpty, Nothing, files.First.Value.First)

            args("upload") = If(file.IsNullOrEmpty, firstFile, file.First)
        End If

        Dim argSet As InvokeParameter() = args _
            .Select(Function(a, i) New InvokeParameter(a.Key, a.Value, i + 1)) _
            .ToArray
        Dim result As Object = func.Invoke(env, argSet)

        If TypeOf result Is FileInfo Then
            ' make file download
            Dim file As FileInfo = DirectCast(result, FileInfo)
            Dim path As String = file.FullName

            Call response.SendFile(path)
        Else
            Call handleResultBuffer(result, req, response, env)
        End If

        Call response.Flush()

        Return Nothing
    End Function

    Private Sub handleResultBuffer(result As Object, req As HttpRequest, response As HttpResponse, env As Environment)
        Dim buffer As BufferObject

        If result Is Nothing Then
            buffer = NullObject.nothing
        ElseIf TypeOf result Is Message Then
            buffer = New messageBuffer(DirectCast(result, Message))
        ElseIf TypeOf result Is list Then
            buffer = New listBuffer(result, env)
        ElseIf TypeOf result Is dataframe Then
            buffer = New dataframeBuffer(result, env)
        ElseIf TypeOf result Is MemoryStream Then
            buffer = New rawBuffer() With {.buffer = DirectCast(result, MemoryStream)}
        ElseIf TypeOf result Is Byte() Then
            buffer = New rawBuffer() With {.buffer = New MemoryStream(DirectCast(result, Byte()))}
        ElseIf TypeOf result Is vector OrElse result.GetType.IsArray Then
            ' processing of the string array?
            If TypeOf result Is vector Then
                If DirectCast(result, vector).elementType Is RType.character Then
                    buffer = New textBuffer(CLRVector.asCharacter(result).JoinBy(vbCrLf))
                Else
                    buffer = vectorBuffer.CreateBuffer(result, env)
                End If
            Else
                If TypeOf result Is String() Then
                    buffer = New textBuffer(CLRVector.asCharacter(result).JoinBy(vbCrLf))
                ElseIf TypeOf result Is Char() Then
                    buffer = New textBuffer(New String(DirectCast(result, Char())))
                Else
                    buffer = vectorBuffer.FromArray(result, env)
                End If
            End If
        ElseIf TypeOf result Is Bitmap Then
            buffer = New bitmapBuffer(DirectCast(result, Bitmap))
        ElseIf TypeOf result Is Image Then
            buffer = New bitmapBuffer(DirectCast(result, Image))
        ElseIf TypeOf result Is String Then
            buffer = New textBuffer(CStr(result))
        Else
            buffer = New textBuffer(CStr(jsonlite.toJSON(result, env, unicode_escape:=False)), "application/json")
        End If

        Call RCallbackMessage.SendHttpResponseMessage(buffer, req, response, debug:=False, showErr:=True)
    End Sub

    ''' <summary>
    ''' parse a closure expression as a router 
    ''' </summary>
    ''' <param name="exp">
    ''' should be a closure expression that contains a list of function declaration
    ''' </param>
    ''' <param name="env">
    ''' the environment that used to parse the closure expression
    ''' </param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    Public Shared Function Parse(exp As Expression, env As Environment) As Object
        Dim funcs As Expression()

        If TypeOf exp Is ClosureExpression Then
            funcs = DirectCast(exp, ClosureExpression) _
                .EnumerateCodeLines _
                .ToArray
        Else
            Return 500
        End If

        Dim urls As New Router

        For Each line As Expression In funcs
            If TypeOf line Is DeclareNewFunction Then
                Dim func As DeclareNewFunction = line
                Dim method As String = func _
                    .GetAttributeValue("http") _
                    .DefaultFirst("get")

                For Each url As String In func.GetAttributeValue("url")
                    Call urls.SetUrl(url, method, func)
                Next
            End If
        Next

        Return urls
    End Function

End Class
