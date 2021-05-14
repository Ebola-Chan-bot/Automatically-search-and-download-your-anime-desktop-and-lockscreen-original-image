Imports Microsoft.Toolkit.Uwp.Notifications

Module 结果与通知
	Public Const 图像尺寸不满足设置的条件 As String = "图像尺寸不满足设置的条件"
	Public Const 下载保存成功 As String = "下载保存成功"
	Public Const 设置的下载目录中已有此图 As String = "设置的下载目录中已有此图。如果没有，请重置哈希"
	Public Const 网站访问失败 As String = "网站访问失败，是否需要翻墙？"
	Public Const 图片下载失败 As String = "图片下载失败，是否需要翻墙？"
	Public Const 暂不支持该网站 As String = "暂不支持该网站，请点击链接手动下载"
	Public Const 网页解析失败 As String = "网页解析失败，没有找到下载链接，请点击链接手动下载"
	Public Const 无法定位当前壁纸 As String = "无法定位当前壁纸，请检查设置是否正确"
	Public Const 无符合要求的原图 As String = "无符合要求的原图"
	Public Const 桌面 As String = "桌面："
	Public Const 锁屏 As String = "锁屏："
	Public ReadOnly 桌面壁纸搜索失败通知 As ToastContentBuilder = 通知初始化(桌面 & 无法定位当前壁纸)
	Public ReadOnly 桌面网站访问失败通知 As ToastContentBuilder = 通知初始化(桌面 & 网站访问失败)
	Public ReadOnly 桌面图片下载失败通知 As ToastContentBuilder = 通知初始化(桌面 & 图片下载失败)
	Public ReadOnly 桌面暂不支持该网站通知 As ToastContentBuilder = 通知初始化(桌面 & 暂不支持该网站)
	Public ReadOnly 桌面网页解析失败通知 As ToastContentBuilder = 通知初始化(桌面 & 网页解析失败)
	Public ReadOnly 锁屏壁纸搜索失败通知 As ToastContentBuilder = 通知初始化(锁屏 & 无法定位当前壁纸)
	Public ReadOnly 锁屏网站访问失败通知 As ToastContentBuilder = 通知初始化(锁屏 & 网站访问失败)
	Public ReadOnly 锁屏图片下载失败通知 As ToastContentBuilder = 通知初始化(锁屏 & 图片下载失败)
	Public ReadOnly 锁屏暂不支持该网站通知 As ToastContentBuilder = 通知初始化(锁屏 & 暂不支持该网站)
	Public ReadOnly 锁屏网页解析失败通知 As ToastContentBuilder = 通知初始化(锁屏 & 网页解析失败)

	Private Function 通知初始化(文本 As String) As ToastContentBuilder
		通知初始化 = New ToastContentBuilder
		通知初始化.AddText(文本)
	End Function
	Sub 自定义通知(文本 As String)
		Dim 通知 As New ToastContentBuilder
		通知.AddText(文本)
		通知.Show()
	End Sub
End Module
