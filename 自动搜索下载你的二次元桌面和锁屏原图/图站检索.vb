Imports System.IO
Imports IqdbApi
Imports IqdbApi.Models
Imports Aspose
Enum 保存结果 As Byte
	未定义
	成功
	重复
	网站访问失败
	图片下载失败
	不支持该网站
	没有找到链接
End Enum

Friend Module 图站检索
	Private ReadOnly IQDB客户端 As New IqdbClient '(New Net.Http.HttpClientHandler With {.ServerCertificateCustomValidationCallback = Function() True})
	Private ReadOnly HTTP客户端 As New Net.Http.HttpClient

	Public Async Function IQDB查询(图片路径 As String) As Task(Of IEnumerable(Of 搜索结果))
		Using 文件流 As FileStream = File.OpenRead(图片路径)
			Return From 结果 As Match In (Await IQDB客户端.SearchFile(文件流)).Matches Select New 搜索结果(结果)
		End Using
	End Function

	Public Async Function 从网页保存原图(网页URL As String) As Task(Of 保存结果)
		Dim HTML文档 As Html.HTMLDocument, 成功 As Boolean
		If 网页URL.Contains("e-shuushuu") Then
			Try
				HTML文档 = Await Task.Run(Function() New Html.HTMLDocument(网页URL))
			Catch ex As Exception
				Return 保存结果.网站访问失败
			End Try
			Dim HTML集合 = HTML文档.GetElementsByClassName("thumb_image")
			If HTML集合.Any Then
				Dim HTML元素 As Html.HTMLAnchorElement = HTML集合.Single
				Dim href = HTML元素.Href
				Dim 文件URL = New Uri(New Uri(HTML元素.BaseURI), href)
				Try
					成功 = Await 尝试入库(Await HTTP客户端.GetByteArrayAsync(文件URL), If(设置项.扩展名一律设为PNG, Path.GetFileNameWithoutExtension(href) + ".png", Path.GetFileName(href)))
				Catch ex As Net.Http.HttpRequestException
					Return 保存结果.图片下载失败
				End Try
				Return If(成功, 保存结果.成功, 保存结果.重复)
			Else
				Return 保存结果.没有找到链接
			End If
		ElseIf 网页URL.Contains("konachan") Then
			Try
				HTML文档 = Await Task.Run(Function() New Html.HTMLDocument(网页URL))
			Catch ex As Exception
				Return 保存结果.网站访问失败
			End Try
			'''解析测试用例：
			'''https://konachan.net/post/show/338743
			Dim HTML集合 = HTML文档.GetElementsByClassName("original-file-unchanged")
			If HTML集合.Any Then
				Dim 文件URL As String = DirectCast(HTML集合.Single, Html.HTMLAnchorElement).Href
				Try
					成功 = Await 尝试入库(Await HTTP客户端.GetByteArrayAsync(文件URL), If(设置项.扩展名一律设为PNG, Path.GetFileNameWithoutExtension(文件URL) + ".png", Path.GetFileName(文件URL)))
				Catch ex As Net.Http.HttpRequestException
					Return 保存结果.图片下载失败
				End Try
				Return If(成功, 保存结果.成功, 保存结果.重复)
			End If
			Dim HTML元素 = HTML文档.GetElementById("highres")
			If HTML元素 IsNot Nothing Then
				Dim 文件URL As String = DirectCast(HTML元素, Html.HTMLAnchorElement).Href
				Try
					成功 = Await 尝试入库(Await HTTP客户端.GetByteArrayAsync(文件URL), If(设置项.扩展名一律设为PNG, Path.GetFileNameWithoutExtension(文件URL) + ".png", Path.GetFileName(文件URL)))
				Catch ex As Net.Http.HttpRequestException
					Return 保存结果.图片下载失败
				End Try
				Return If(成功, 保存结果.成功, 保存结果.重复)
			End If
			Return 保存结果.没有找到链接
		ElseIf 网页URL.Contains("zerochan") Then
			Try
				HTML文档 = Await Task.Run(Function() New Html.HTMLDocument(网页URL))
			Catch ex As Exception
				Return 保存结果.网站访问失败
			End Try
			Dim HTML集合 = HTML文档.GetElementsByClassName("preview")
			If HTML集合.Any Then
				Dim 文件URL = DirectCast(HTML集合.Single, Html.HTMLAnchorElement).Href
				Try
					成功 = Await 尝试入库(Await HTTP客户端.GetByteArrayAsync(文件URL), If(设置项.扩展名一律设为PNG, Path.GetFileNameWithoutExtension(文件URL) + ".png", Path.GetFileName(文件URL)))
				Catch ex As Net.Http.HttpRequestException
					Return 保存结果.图片下载失败
				End Try
				Return If(成功, 保存结果.成功, 保存结果.重复)
			Else
				Return 保存结果.没有找到链接
			End If
		Else
			Return 保存结果.不支持该网站
		End If
	End Function

	Public Async Function 搜索并筛选原图(路径 As String) As Task(Of 保存结果)
		Dim 原始结果 As SearchResult
		Dim 缩略图哈希 = 计算哈希(File.ReadAllBytes(路径))
		If 检查库存(缩略图哈希) Then
			Return 保存结果.重复
		Else
			Using 文件流 As FileStream = File.OpenRead(路径)
				原始结果 = Await IQDB客户端.SearchFile(文件流)
			End Using
		End If
		Dim 筛选列表 As IEnumerable(Of (Match, UInteger, UInteger)) = From 结果 As Match In 原始结果.Matches Where 结果.Resolution.Width >= 设置项.宽度不小于 AndAlso 结果.Resolution.Height >= 设置项.高度不小于 AndAlso 结果.Similarity >= 设置项.相似度不小于 AndAlso (结果.MatchType = Enums.MatchType.Best OrElse 结果.MatchType = Enums.MatchType.Additional) Select (结果, Math.Min(结果.Resolution.Height, 结果.Resolution.Width), Math.Max(结果.Resolution.Height, 结果.Resolution.Width))
		If 筛选列表.Any = False Then
			Return 保存结果.未定义
		End If
		Dim 最大短轴 As UInteger = Aggregate 结果 As (Match, UInteger, UInteger) In 筛选列表 Into Max(结果.Item2)
		筛选列表 = From 结果 As (Match, UInteger, UInteger) In 筛选列表 Where 结果.Item2 >= 最大短轴
		If 筛选列表.Any = False Then
			Return 保存结果.未定义
		End If
		Dim 最大长轴 As UInteger = Aggregate 结果 As (Match, UInteger, UInteger) In 筛选列表 Into Max(结果.Item3)
		Dim Match列表 As IEnumerable(Of Match) = From 结果 As (Match, UInteger, UInteger) In 筛选列表 Where 结果.Item3 >= 最大长轴 Select 结果.Item1
		Dim 原图尺寸 As Resolution = 原始结果.YourImage.Resolution
		Dim 结果列表 = If(原图尺寸.Width >= 原图尺寸.Height,
			From 结果 As Match In Match列表 Where 结果.Resolution.Width >= 结果.Resolution.Height,
			From 结果 As Match In Match列表 Where 结果.Resolution.Width < 结果.Resolution.Height)
		If Not 结果列表.Any Then
			结果列表 = Match列表
		End If
		Dim 最终结果 As 保存结果 = 保存结果.未定义, 当前结果 As 保存结果
		For Each URL As String In From 结果 As Match In 结果列表 Select 搜索结果.URL修饰(结果.Url)
			当前结果 = Await 从网页保存原图(URL)
			Select Case 当前结果
				Case 保存结果.成功
					最终结果 = 保存结果.成功
					缩略图入库(缩略图哈希)
					Exit For
				Case 保存结果.重复
					最终结果 = 保存结果.重复
					缩略图入库(缩略图哈希)
					Exit For
				Case Else
					If 最终结果 = 保存结果.未定义 Then
						最终结果 = 当前结果
					End If
			End Select
		Next
		Return 最终结果
	End Function
End Module
