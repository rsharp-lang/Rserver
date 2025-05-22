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
<RTypeExport("router", GetType(Router))>
Module RouterFunction

    <ExportAPI("new")>
    Public Function newRouter() As Router
        Return New Router
    End Function

    <ExportAPI("parse")>
    Public Function parse(<RLazyExpression> exp As Expression, Optional env As Environment = Nothing) As Object
        Return Router.Parse(exp, env)
    End Function

    ''' <summary>
    ''' handle the http request
    ''' </summary>
    ''' <param name="req"></param>
    ''' <param name="response"></param>
    ''' <param name="router"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("handle")>
    Public Function handle(router As Router, req As HttpRequest, response As HttpResponse, Optional env As Environment = Nothing) As Object
        Dim println = env.WriteLineHandler
        println("start to handle http request!")
        Return router.HandleRequest(req, response, env)
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="router"></param>
    ''' <param name="url"></param>
    ''' <param name="method">the http method: get/post</param>
    ''' <param name="handler"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("register_url")>
    Public Function register_url(router As Router, url As String, method As String, handler As DeclareNewFunction, Optional env As Environment = Nothing) As Object
        Call router.SetUrl(url, method, handler)
        Return router
    End Function

    <ExportAPI("check_url")>
    Public Function check_url(router As Router, req As HttpRequest) As Boolean
        Dim url = req.URL.path
        Dim flag As Boolean = router.CheckUrl(url)
        Return flag
    End Function
End Module
