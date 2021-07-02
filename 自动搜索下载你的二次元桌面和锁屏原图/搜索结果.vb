Class 搜索结果
	Private Shared ReadOnly 匹配类型翻译 As String() = {"最好", "额外", "可能", "其它"}
	Private Shared ReadOnly 评级翻译 As String() = {"未分级", "安全", "可疑", "明确"}
	Private Shared ReadOnly 源翻译 As String() = {"Danbooru", "Konachan", "Yandere", "Gelbooru", "SankakuChannel", "Eshuushuu", "TheAnimeGallery", "Zerochan", "AnimePictures"}
	Property 匹配类型 As String
	Property 预览URL As String
	Property 评级 As String
	Property 高度 As UShort
	Property 宽度 As UShort
	Property 得分 As Byte?
	Property 相似 As Byte
	Property 源 As String
	Property URL As String
	Protected Sub New()
	End Sub
	Sub New(结果 As IqdbApi.Models.Match)
		匹配类型 = 匹配类型翻译(结果.MatchType)
		评级 = 评级翻译(结果.Rating)
		高度 = 结果.Resolution.Height
		宽度 = 结果.Resolution.Width
		得分 = 结果.Score
		相似 = 结果.Similarity
		源 = 源翻译(结果.Source)
		预览URL = "https://www.iqdb.org" & 结果.PreviewUrl
		URL = URL修饰(结果.Url)
	End Sub
	Public Shared Function URL修饰(URL As String) As String
		If Not URL.StartsWith("http") Then
			URL = "https:" & URL
		End If
		If 设置项.重定向Konachan Then
			URL = URL.Replace("konachan.com", "konachan.net", StringComparison.OrdinalIgnoreCase)
		End If
		Return URL
	End Function
End Class