#Region "Microsoft.VisualBasic::29c3959bada69291cced91415a241f91, win32_desktop\src\Rstudio\Rserver\src\Router.vb"

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

    '   Total Lines: 72
    '    Code Lines: 52 (72.22%)
    ' Comment Lines: 5 (6.94%)
    '    - Xml Docs: 60.00%
    ' 
    '   Blank Lines: 15 (20.83%)
    '     File Size: 2.27 KB


    ' Module RouterFunction
    ' 
    '     Function: handle, parse
    ' 
    ' Class Router
    ' 
    '     Function: HandleRequest, Parse
    ' 
    ' /********************************************************************************/

#End Region

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
        Dim println = env.WriteLineHandler
        println("start to handle http request!")
        Return router.HandleRequest(req, response, env)
    End Function
End Module

Public Class Router

    ReadOnly urls As New Dictionary(Of String, DeclareNewFunction)

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
