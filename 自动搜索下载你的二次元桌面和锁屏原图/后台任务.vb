Imports Microsoft.Win32
Imports Windows.ApplicationModel
Imports System.Threading

Module 后台任务
	Property 日志流 As IO.StreamWriter
	Sub 设置后台任务()
		Const 启动参数 As String = "后台任务", 应用标识 As String = "自动搜索下载你的桌面和锁屏原图"
		Static 任务服务 As TaskScheduler.TaskService = TaskScheduler.TaskService.Instance, 计划任务 As TaskScheduler.Task, 开机启动 As StartupTask = StartupTask.GetAsync("自启动任务").GetResults, 定时器 As New Timer(AddressOf 定时任务), 启动路径 As String = Process.GetCurrentProcess.MainModule.FileName
		Select Case 设置项.每隔时间单位
			Case 时间单位.关闭
				定时器.Change(Timeout.Infinite, Timeout.Infinite)
				开机启动.Disable()
				If 计划任务 IsNot Nothing Then
					计划任务.Enabled = False
				End If
			Case 时间单位.分钟
				Dim 时间间隔 As TimeSpan = TimeSpan.FromMinutes(时间值(时间单位.分钟)(设置项.每隔时间值))
				Dim 距离上次 As TimeSpan = Now - 设置项.上次任务时日
				If 时间间隔 > 距离上次 Then
					定时器.Change(时间间隔 - 距离上次, 时间间隔)
				Else
					定时器.Change(TimeSpan.Zero, 时间间隔)
				End If
				If 计划任务 IsNot Nothing Then
					计划任务.Enabled = False
				End If
				Call 开机启动.RequestEnableAsync()
			Case 时间单位.小时
				Dim 时间间隔 As TimeSpan = TimeSpan.FromMinutes(时间值(时间单位.小时)(设置项.每隔时间值))
				Dim 距离上次 As TimeSpan = Now - 设置项.上次任务时日
				If 时间间隔 > 距离上次 Then
					定时器.Change(时间间隔 - 距离上次, 时间间隔)
				Else
					定时器.Change(TimeSpan.Zero, 时间间隔)
				End If
				If 计划任务 IsNot Nothing Then
					计划任务.Enabled = False
				End If
				Call 开机启动.RequestEnableAsync()
			Case 时间单位.天
				定时器.Change(Timeout.Infinite, Timeout.Infinite)
				开机启动.Disable()
				If IsNothing(计划任务) Then
					计划任务 = 任务服务.AddTask(应用标识, New TaskScheduler.DailyTrigger(时间值(时间单位.天)(设置项.每隔时间值)), New TaskScheduler.ExecAction(启动路径, 启动参数))
				End If
				计划任务.Enabled = True
		End Select
	End Sub
	Sub 写日志(内容 As String)
		If 日志流 IsNot Nothing Then
			日志流.WriteLine(Now.ToString & " " & 内容)
			日志流.Flush()
		End If
	End Sub
	Async Sub 定时任务()
		Dim 路径 As String = 搜索桌面壁纸(设置项.当前桌面路径)
		Dim 通知文本 As String
		If String.IsNullOrEmpty(路径) Then
			发通知(桌面壁纸搜索失败通知)
			写日志(桌面 & 无法定位当前壁纸)
		Else
			Try
				Select Case Await 搜索并筛选原图(路径)
					Case 保存结果.成功
						写日志(桌面 & 下载保存成功)
					Case 保存结果.重复
						写日志(桌面 & 设置的下载目录中已有此图)
					Case 保存结果.网站访问失败
						发通知(桌面网站访问失败通知)
						写日志(桌面 & 网站访问失败)
					Case 保存结果.图片下载失败
						发通知(桌面图片下载失败通知)
						写日志(桌面 & 图片下载失败)
					Case 保存结果.不支持该网站
						发通知(桌面暂不支持该网站通知)
						写日志(桌面 & 暂不支持该网站)
					Case 保存结果.没有找到链接
						发通知(桌面网页解析失败通知)
						写日志(桌面 & 网页解析失败)
					Case 保存结果.未定义
						写日志(桌面 & 无符合要求的原图)
				End Select
			Catch ex As IqdbApi.Exceptions.InvalidFileFormatException
				通知文本 = 桌面 & If(IsNothing(ex.InnerException), ex.Message, ex.InnerException.Message)
				发通知(通知文本)
				写日志(通知文本)
			Catch ex As Net.Http.HttpRequestException
				通知文本 = 桌面 & ex.InnerException.Message
				发通知(通知文本)
				写日志(通知文本)
			End Try
		End If
		路径 = 搜索锁屏壁纸(设置项.当前锁屏路径)
		If String.IsNullOrEmpty(路径) Then
			发通知(锁屏壁纸搜索失败通知)
			写日志(锁屏 & 无法定位当前壁纸)
		Else
			Try
				Select Case Await 搜索并筛选原图(路径)
					Case 保存结果.成功
						写日志(锁屏 & 下载保存成功)
					Case 保存结果.重复
						写日志(锁屏 & 设置的下载目录中已有此图)
					Case 保存结果.网站访问失败
						发通知(锁屏网站访问失败通知)
						写日志(锁屏 & 网站访问失败)
					Case 保存结果.图片下载失败
						发通知(锁屏图片下载失败通知)
						写日志(锁屏 & 图片下载失败)
					Case 保存结果.不支持该网站
						发通知(锁屏暂不支持该网站通知)
						写日志(锁屏 & 暂不支持该网站)
					Case 保存结果.没有找到链接
						发通知(锁屏网页解析失败通知)
						写日志(锁屏 & 网页解析失败)
					Case 保存结果.未定义
						写日志(锁屏 & 无符合要求的原图)
				End Select
			Catch ex As IqdbApi.Exceptions.InvalidFileFormatException
				通知文本 = 锁屏 & If(IsNothing(ex.InnerException), ex.Message, ex.InnerException.Message)
				发通知(通知文本)
				写日志(通知文本)
			Catch ex As Net.Http.HttpRequestException
				通知文本 = 锁屏 & ex.InnerException.Message
				发通知(通知文本)
				写日志(通知文本)
			End Try
		End If
	End Sub
End Module
