Imports System.Net.Http.Headers
Imports Flute.Http.Core.Message
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.Rsharp.Interpreter.ExecuteEngine
Imports SMRUCC.Rsharp.Interpreter.ExecuteEngine.ExpressionSymbols.Closure
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Interop

''' <summary>
''' the http url router
''' </summary>
''' 
<Package("router")>
Module RouterFunction

    <ExportAPI("parse")>
    Public Function parse(<RLazyExpression> exp As Expression, Optional env As Environment = Nothing) As Object
        Return Router.Parse(exp, env)
    End Function

    <ExportAPI("handle")>
    Public Function handle(req As HttpRequest, response As HttpResponse, router As Router, Optional env As Environment = Nothing) As Object
        Return router.HandleRequest(req, response, env)
    End Function
End Module

Public Class Router

    ReadOnly urls As New Dictionary(Of String, DeclareNewFunction)

    Public Function HandleRequest(req As HttpRequest, response As HttpResponse, env As Environment) As Object
        Dim url = req.URL
        Dim func = urls.TryGetValue(url.path)

        If func Is Nothing Then
            ' 404
            Return 404
        Else
            Return func.Invoke({req, response}, caller:=env)
        End If
    End Function

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