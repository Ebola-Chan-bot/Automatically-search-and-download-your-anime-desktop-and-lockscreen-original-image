Imports System.ComponentModel

Class 结果显示
	Inherits 搜索结果
	Implements INotifyPropertyChanged
	ReadOnly Property 自己 As 结果显示 = Me
	Private i工作中 As Boolean = False
	Private i结果提示 As String = ""
	Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged
	Property 工作中 As Boolean
		Get
			Return i工作中
		End Get
		Set(value As Boolean)
			Static 事件参数 As New PropertyChangedEventArgs(NameOf(工作中))
			i工作中 = value
			RaiseEvent PropertyChanged(Me, 事件参数)
		End Set
	End Property
	Property 结果提示 As String
		Get
			Return i结果提示
		End Get
		Set(value As String)
			Static 事件参数 As New PropertyChangedEventArgs(NameOf(结果提示))
			i结果提示 = value
			RaiseEvent PropertyChanged(Me, 事件参数)
		End Set
	End Property
	Sub New(原版 As 搜索结果)
		With 原版
			URL = .URL
			匹配类型 = .匹配类型
			宽度 = .宽度
			得分 = .得分
			源 = .源
			相似 = .相似
			评级 = .评级
			预览URL = .预览URL
			高度 = .高度
		End With
	End Sub
End Class