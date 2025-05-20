Imports System.Runtime.CompilerServices
Imports Flute.Http.Core.Message
Imports SMRUCC.Rsharp.Interpreter.ExecuteEngine
Imports SMRUCC.Rsharp.Interpreter.ExecuteEngine.ExpressionSymbols.Closure
Imports SMRUCC.Rsharp.Runtime

''' <summary>
''' http url router
''' </summary>
''' <remarks>
''' redirect of the http request to the specific runtime function
''' </remarks>
Public Class Router

    ReadOnly urls As New Dictionary(Of String, DeclareNewFunction)

    ''' <summary>
    ''' Check of the url is existed inside current router?
    ''' </summary>
    ''' <param name="url"></param>
    ''' <returns></returns>
    ''' 
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function CheckUrl(url As Boolean) As Boolean
        Return urls.ContainsKey(url)
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Sub SetUrl(url As String, handler As DeclareNewFunction)
        urls(url) = handler
    End Sub

    Public Function HandleRequest(req As HttpRequest, response As HttpResponse, env As Environment) As Object
        Dim url = req.URL
        Dim func = urls.TryGetValue(url.path)
        Dim writeLine = env.WriteLineHandler

        Call writeLine($" -> [{url.path}]")

        If func Is Nothing Then
            ' 404
            Return 404
        Else
            Return func.Invoke({req, response}, caller:=env)
        End If
    End Function

    ''' <summary>
    ''' parse a closure expression as a router 
    ''' </summary>
    ''' <param name="exp"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    Public Shared Function Parse(exp As Expression, env As Environment) As Object
        Dim funcs As Expression()

        If TypeOf exp Is ClosureExpression Then
            funcs = DirectCast(exp, ClosureExpression).EnumerateCodeLines.ToArray
        Else
            Return 500
        End If

        Dim urls As New Router

        For Each line As Expression In funcs
            If TypeOf line Is DeclareNewFunction Then
                Dim func As DeclareNewFunction = line

                For Each url As String In func.GetAttributeValue("url")
                    urls.urls(url) = func
                Next
            End If
        Next

        Return urls
    End Function

End Class
