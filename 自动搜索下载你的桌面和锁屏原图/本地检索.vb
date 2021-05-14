Imports System.IO
Module 本地检索
	Function 搜索桌面壁纸(目录 As String) As String
		搜索桌面壁纸 = (From 文件 In Directory.GetFiles(目录, "*.jpg") Order By File.GetLastWriteTime(文件) Descending).FirstOrDefault
		If String.IsNullOrEmpty(搜索桌面壁纸) Then
			Return (From 文件 In Directory.GetFiles(目录, "*.png") Order By File.GetLastWriteTime(文件) Descending).FirstOrDefault
		End If
	End Function
	Function 搜索锁屏壁纸(目录 As String) As String
		搜索锁屏壁纸 = 搜索桌面壁纸(目录)
		If String.IsNullOrEmpty(搜索锁屏壁纸) Then
			Return (From 路径 In Directory.GetDirectories(目录) Order By Directory.GetLastWriteTime(路径) Descending Select 文件 = Path.Combine(路径, "LockScreen.jpg") Where File.Exists(文件)).FirstOrDefault()
		End If
	End Function
	Async Function 读入位图(路径 As String) As Task(Of BitmapFrame)
		Dim 文件流 As Stream = File.OpenRead(路径)
		Dim 内存流 As New MemoryStream
		Await File.OpenRead(路径).CopyToAsync(内存流)
		文件流.Close()
		内存流.Position = 0
		Return BitmapDecoder.Create(内存流, BitmapCreateOptions.None, BitmapCacheOption.OnLoad).Frames(0)
	End Function
End Module
