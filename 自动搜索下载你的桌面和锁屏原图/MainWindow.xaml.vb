Imports System.ComponentModel

Class MainWindow
	Private 桌面文件路径 As String, 锁屏文件路径 As String
	ReadOnly 桌面_高亮提示错误 As Animation.Storyboard = Resources.Item("桌面_高亮提示错误"), 锁屏_高亮提示错误 As Animation.Storyboard = Resources.Item("锁屏_高亮提示错误")
	Sub New()

		' 此调用是设计器所必需的。
		InitializeComponent()

		' 在 InitializeComponent() 调用之后添加任何初始化。

		设置桌面路径(设置项.当前桌面路径)
		设置锁屏路径(设置项.当前锁屏路径)
		下载保存路径.Text = 设置项.下载保存路径
		每隔时间单位.SelectedIndex = 设置项.每隔时间单位
		每隔时间值.SelectedIndex = 设置项.每隔时间值
		宽度不小于.Value = 设置项.宽度不小于
		高度不小于.Value = 设置项.高度不小于
		重定向Konachan.IsChecked = 设置项.重定向Konachan
		后台任务异常时发送通知.IsChecked = 设置项.后台任务异常时发送通知
		保存后台日志.IsChecked = 设置项.保存后台日志
		后台日志路径.Text = 设置项.后台日志路径
		DirectCast(System.Windows.Application.Current, Application).窗口打开了(Me)
	End Sub
	Private Sub 每隔时间单位_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles 每隔时间单位.SelectionChanged
		每隔时间值.ItemsSource = 时间值(每隔时间单位.SelectedIndex)
		设置项.每隔时间单位 = 每隔时间单位.SelectedIndex
		Task.Run(AddressOf 设置后台任务)
	End Sub
	Private Sub 设置桌面路径(路径 As String)
		设置项.当前桌面路径 = 路径
		桌面_当前搜索路径.Text = 路径
		刷新桌面图片()
	End Sub
	Private Sub 每隔时间值_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles 每隔时间值.SelectionChanged
		If 每隔时间值.SelectedIndex = -1 Then
			设置项.每隔时间值 = 0
		Else
			设置项.每隔时间值 = 每隔时间值.SelectedIndex
		End If
	End Sub
	Private Sub 设置锁屏路径(路径 As String)
		设置项.当前锁屏路径 = 路径
		锁屏_当前搜索路径.Text = 路径
		刷新锁屏图片()
	End Sub
	Private Sub 宽度不小于_ValueChanged(sender As Object, e As RoutedPropertyChangedEventArgs(Of Double)) Handles 宽度不小于.ValueChanged
		设置项.宽度不小于 = 宽度不小于.Value
	End Sub
	Private Async Sub 刷新桌面图片() Handles 桌面_当前图片.MouseUp
		桌面文件路径 = 搜索桌面壁纸(设置项.当前桌面路径)
		If String.IsNullOrEmpty(桌面文件路径) Then
			桌面_当前图片.Visibility = Visibility.Collapsed
			桌面_尺寸栏.Visibility = Visibility.Collapsed
			桌面图片错误.Text = "指定路径未找到JPG/PNG图像文件"
			桌面图片错误.Visibility = Visibility.Visible
		Else
			桌面图片错误.Visibility = Visibility.Collapsed
			Dim 桌面位图 As BitmapFrame = Await 读入位图(桌面文件路径)
			桌面_当前图片.Source = 桌面位图
			桌面_宽度.Text = 桌面位图.PixelWidth
			桌面_高度.Text = 桌面位图.PixelHeight
			桌面_当前图片.Visibility = Visibility.Visible
			桌面_尺寸栏.Visibility = Visibility.Visible
		End If
	End Sub
	Private Sub 高度不小于_ValueChanged(sender As Object, e As RoutedPropertyChangedEventArgs(Of Double)) Handles 高度不小于.ValueChanged
		设置项.高度不小于 = 高度不小于.Value
	End Sub
	Private Async Sub 刷新锁屏图片() Handles 锁屏_当前图片.MouseUp
		锁屏文件路径 = 搜索锁屏壁纸(设置项.当前锁屏路径)
		If String.IsNullOrEmpty(锁屏文件路径) Then
			锁屏_当前图片.Visibility = Visibility.Collapsed
			锁屏_尺寸栏.Visibility = Visibility.Collapsed
			锁屏图片错误.Text = "指定路径未找到锁屏图像文件"
			锁屏图片错误.Visibility = Visibility.Visible
		Else
			锁屏图片错误.Visibility = Visibility.Collapsed
			Dim 锁屏位图 As BitmapFrame = Await 读入位图(锁屏文件路径)
			锁屏_当前图片.Source = 锁屏位图
			锁屏_宽度.Text = 锁屏位图.PixelWidth
			锁屏_高度.Text = 锁屏位图.PixelHeight
			锁屏_当前图片.Visibility = Visibility.Visible
			锁屏_尺寸栏.Visibility = Visibility.Visible
		End If
	End Sub
	Private Sub MainWindow_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
		写出设置(设置项)
	End Sub

	Private Sub 桌面_重置默认_Click(sender As Object, e As RoutedEventArgs) Handles 桌面_重置默认.Click
		设置桌面路径(默认桌面路径)
	End Sub

	Private Sub 锁屏_重置默认_Click(sender As Object, e As RoutedEventArgs) Handles 锁屏_重置默认.Click
		设置锁屏路径(默认锁屏路径)
	End Sub

	Private Sub 保存设置_Click(sender As Object, e As RoutedEventArgs) Handles 保存设置.Click
		写出设置(设置项)
	End Sub

	Private Sub 桌面_浏览_Click(sender As Object, e As RoutedEventArgs) Handles 桌面_浏览.Click
		Static 对话框 As New Forms.FolderBrowserDialog With {.SelectedPath = 设置项.当前桌面路径}
		If 对话框.ShowDialog = Forms.DialogResult.OK Then
			设置桌面路径(对话框.SelectedPath)
		End If
	End Sub

	Private Sub 锁屏_浏览_Click(sender As Object, e As RoutedEventArgs) Handles 锁屏_浏览.Click
		Static 对话框 As New Forms.FolderBrowserDialog With {.SelectedPath = 设置项.当前锁屏路径}
		If 对话框.ShowDialog = Forms.DialogResult.OK Then
			设置锁屏路径(对话框.SelectedPath)
		End If
	End Sub
	Private Async Sub 桌面_IQDB搜索_Click(sender As Object, e As RoutedEventArgs) Handles 桌面_IQDB搜索.Click
		If String.IsNullOrEmpty(桌面文件路径) Then
			桌面_高亮提示错误.Begin()
		Else
			桌面_搜索进度.IsIndeterminate = True
			Try
				桌面_结果列表.ItemsSource = From 结果 As 搜索结果 In Await IQDB查询(桌面文件路径) Select New 结果显示(结果)
			Catch ex As IqdbApi.Exceptions.InvalidFileFormatException
				桌面_错误信息.Text = ex.InnerException.Message
			Catch ex As Exception
				桌面_错误信息.Text = ex.Message
			End Try
			桌面_搜索进度.IsIndeterminate = False
		End If
	End Sub
	Private Async Sub 锁屏_IQDB搜索_Click(sender As Object, e As RoutedEventArgs) Handles 锁屏_IQDB搜索.Click
		If String.IsNullOrEmpty(锁屏文件路径) Then
			锁屏_高亮提示错误.Begin()
		Else
			锁屏_搜索进度.IsIndeterminate = True
			Try
				锁屏_结果列表.ItemsSource = From 结果 As 搜索结果 In Await IQDB查询(锁屏文件路径) Select New 结果显示(结果)
			Catch ex As IqdbApi.Exceptions.InvalidFileFormatException
				锁屏_错误信息.Text = ex.InnerException.Message
			Catch ex As Exception
				锁屏_错误信息.Text = ex.Message
			End Try
			锁屏_搜索进度.IsIndeterminate = False
		End If
	End Sub

	Private Sub Hyperlink_RequestNavigate(sender As Object, e As RequestNavigateEventArgs)
		Process.Start(New ProcessStartInfo(DirectCast(sender, Hyperlink).NavigateUri.AbsoluteUri) With {.UseShellExecute = True})
	End Sub

	Private Sub 浏览下载路径_Click(sender As Object, e As RoutedEventArgs) Handles 浏览下载路径.Click
		Static 对话框 As New Forms.FolderBrowserDialog With {.SelectedPath = 设置项.下载保存路径}
		If 对话框.ShowDialog = Forms.DialogResult.OK Then
			设置项.下载保存路径 = 对话框.SelectedPath
			下载保存路径.Text = 设置项.下载保存路径
		End If
	End Sub

	Private Sub 重定向Konachan_Checked(sender As Object, e As RoutedEventArgs) Handles 重定向Konachan.Checked, 重定向Konachan.Unchecked
		设置项.重定向Konachan = 重定向Konachan.IsChecked
	End Sub

	Private Async Sub Button_Click(sender As Object, e As RoutedEventArgs)
		Dim 结果 As 结果显示 = DirectCast(sender, Button).Tag
		If 结果.宽度 < 设置项.宽度不小于 OrElse 结果.高度 < 设置项.高度不小于 Then
			结果.结果提示 = 图像尺寸不满足设置的条件
		Else
			结果.工作中 = True
			Select Case Await 从网页保存原图(结果.URL)
				Case 保存结果.成功
					结果.结果提示 = 下载保存成功
				Case 保存结果.重复
					结果.结果提示 = 设置的下载目录中已有此图
				Case 保存结果.网站访问失败
					结果.结果提示 = 网站访问失败
				Case 保存结果.图片下载失败
					结果.结果提示 = 图片下载失败
				Case 保存结果.不支持该网站
					结果.结果提示 = 暂不支持该网站
				Case 保存结果.没有找到链接
					结果.结果提示 = 网页解析失败
			End Select
			结果.工作中 = False
		End If
	End Sub

	Private Async Sub 重置哈希_Click(sender As Object, e As RoutedEventArgs) Handles 重置哈希.Click
		哈希生成中.IsIndeterminate = True
		哈希提示.Text = "生成哈希……"
		Await Task.Run(AddressOf 重置哈希表)
		哈希提示.Text = "完成"
		哈希生成中.IsIndeterminate = False
	End Sub

	Private Sub 后台任务异常时发送通知_Checked(sender As Object, e As RoutedEventArgs) Handles 后台任务异常时发送通知.Checked, 后台任务异常时发送通知.Unchecked
		设置项.后台任务异常时发送通知 = 后台任务异常时发送通知.IsChecked
	End Sub

	Private Sub 浏览日志路径_Click(sender As Object, e As RoutedEventArgs) Handles 浏览日志路径.Click
		Static 对话框 As New Microsoft.Win32.SaveFileDialog With {.InitialDirectory = IO.Path.GetDirectoryName(设置项.后台日志路径), .FileName = "日志", .DefaultExt = ".log", .Filter = "日志文件|*.log", .Title = "选择保存位置，已有文件将会追加，不会覆盖", .OverwritePrompt = False}
		If 对话框.ShowDialog Then
			设置项.后台日志路径 = 对话框.FileName
			后台日志路径.Text = 设置项.后台日志路径
			If 设置项.保存后台日志 Then
				日志流.Close()
				日志流 = IO.File.AppendText(设置项.后台日志路径)
			End If
		End If
	End Sub

	Private Sub 保存后台日志_Checked(sender As Object, e As RoutedEventArgs) Handles 保存后台日志.Checked
		Try
			IO.Directory.CreateDirectory(IO.Path.GetDirectoryName(设置项.后台日志路径))
		Catch ex As ArgumentException
			后台日志路径.Text = "日志保存路径无效，将不会保存日志"
			保存后台日志.IsChecked = False
			Exit Sub
		End Try
		设置项.保存后台日志 = True
		If IsNothing(日志流) Then
			日志流 = IO.File.AppendText(设置项.后台日志路径)
		End If
	End Sub

	Private Sub 保存后台日志_Unchecked(sender As Object, e As RoutedEventArgs) Handles 保存后台日志.Unchecked
		设置项.保存后台日志 = False
		If 日志流 IsNot Nothing Then
			日志流.Close()
		End If
	End Sub
End Class
