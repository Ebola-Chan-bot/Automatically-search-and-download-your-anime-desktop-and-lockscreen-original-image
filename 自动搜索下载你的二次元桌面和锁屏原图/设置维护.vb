Imports System.IO
Friend Enum 时间单位 As Byte
	关闭 = 0
	分钟 = 1
	小时 = 2
	天 = 3
End Enum
Friend Structure 设置
	Property 当前桌面路径 As String
	Property 当前锁屏路径 As String
	Property 下载保存路径 As String
	Property 每隔时间单位 As 时间单位
	Property 每隔时间值 As Byte
	Property 宽度不小于 As UShort
	Property 高度不小于 As UShort
	Property 相似度不小于 As Byte
	Property 重定向Konachan As Boolean
	Property 扩展名一律设为PNG As Boolean
	Property 保存后台日志 As Boolean
	Property 后台日志路径 As String
	Property 上次任务时日 As Date
	Property 后台任务异常时发送通知 As Boolean
End Structure
Module 设置维护
	ReadOnly 设置目录 As String = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)
	ReadOnly 设置路径 As String = Path.Combine(设置目录, "设置.bin")
	ReadOnly Property 默认桌面路径 As String = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Microsoft\Windows\Themes\CachedFiles")
	ReadOnly Property 默认锁屏路径 As String = Path.Combine("C:\ProgramData\Microsoft\Windows\SystemData", Security.Principal.WindowsIdentity.GetCurrent.User.Value, "ReadOnly")
	Friend 设置项 As 设置
	ReadOnly Property 时间值 As Byte()() = {Array.Empty(Of Byte)(), New Byte() {1, 2, 4, 8, 15, 30}, New Byte() {1, 2, 3, 6, 12}, New Byte() {1, 2, 4, 7}}

	Sub 写出设置(设置内容 As 设置)
		Directory.CreateDirectory(设置目录)
		Using 写出器 As New BinaryWriter(File.OpenWrite(设置路径))
			写出器.Write(设置内容.当前桌面路径)
			写出器.Write(设置内容.当前锁屏路径)
			写出器.Write(设置内容.下载保存路径)
			写出器.Write(设置内容.每隔时间单位)
			写出器.Write(设置内容.每隔时间值)
			写出器.Write(设置内容.宽度不小于)
			写出器.Write(设置内容.高度不小于)
			写出器.Write(设置内容.相似度不小于)
			写出器.Write(设置内容.重定向Konachan)
			写出器.Write(设置内容.后台任务异常时发送通知)
			写出器.Write(设置内容.保存后台日志)
			写出器.Write(设置内容.后台日志路径)
			写出器.Write(设置内容.上次任务时日.Ticks)
			写出器.Write(设置内容.扩展名一律设为PNG)
		End Using
	End Sub
	Sub 写出设置()
		Static 默认设置 As New 设置 With {.当前桌面路径 = 默认桌面路径, .当前锁屏路径 = 默认锁屏路径, .下载保存路径 = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), .重定向Konachan = True, .后台任务异常时发送通知 = True, .后台日志路径 = "", .上次任务时日 = Now, .扩展名一律设为PNG = True}
		写出设置(默认设置)
	End Sub
	Private Function 读入设置() As 设置
		Using 读入器 As New BinaryReader(File.OpenRead(设置路径))
			Return New 设置 With {.当前桌面路径 = 读入器.ReadString, .当前锁屏路径 = 读入器.ReadString, .下载保存路径 = 读入器.ReadString, .每隔时间单位 = 读入器.ReadByte, .每隔时间值 = 读入器.ReadByte, .宽度不小于 = 读入器.ReadUInt16, .高度不小于 = 读入器.ReadUInt16, .相似度不小于 = 读入器.ReadByte, .重定向Konachan = 读入器.ReadBoolean, .后台任务异常时发送通知 = 读入器.ReadBoolean, .保存后台日志 = 读入器.ReadBoolean, .后台日志路径 = 读入器.ReadString, .上次任务时日 = New Date(读入器.ReadInt64), .扩展名一律设为PNG = 读入器.ReadBoolean}
		End Using
	End Function
	Sub 初始化设置()
		If File.Exists(设置路径) Then
			Try
				设置项 = 读入设置()
			Catch ex As EndOfStreamException
				写出设置()
				设置项 = 读入设置()
			End Try
		Else
			写出设置()
			设置项 = 读入设置()
		End If
	End Sub
End Module