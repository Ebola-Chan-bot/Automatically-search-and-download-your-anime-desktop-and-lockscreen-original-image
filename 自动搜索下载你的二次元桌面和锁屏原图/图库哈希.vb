Imports System.Security.Cryptography
Imports System.IO
Imports System.Diagnostics.CodeAnalysis

Module 图库哈希
	Private 哈希表 As New List(Of Byte())
	Private 哈希任务 As Task
	Private 哈希流 As FileStream
	Const 文件名 As String = "哈希表.sha256"
	Private Function 新建哈希表(哈希表路径 As String) As Task
		Return Task.Run(Sub()
							哈希表 = (From 路径 In Directory.GetFiles(设置项.下载保存路径, "*.jpg").Concat(Directory.GetFiles(设置项.下载保存路径, "*.png")).AsParallel Select 计算哈希(File.ReadAllBytes(路径))).ToList
							哈希流 = File.OpenWrite(哈希表路径)
							For Each 哈希 As Byte() In 哈希表
								哈希流.Write(哈希)
							Next
						End Sub)
	End Function
	Sub 初始化哈希表()
		哈希任务 = Task.Run(Sub()
							Dim 哈希表路径 As String = Path.Combine(设置项.下载保存路径, 文件名)
							If File.Exists(哈希表路径) Then
								哈希流 = File.OpenRead(哈希表路径)
								Dim 读取器 As New BinaryReader(哈希流)
								For a As UShort = 1 To 哈希流.Length / 32
									哈希表.Add(读取器.ReadBytes(32))
								Next
								读取器.Close()
								哈希流 = File.OpenWrite(哈希表路径)
								哈希流.Seek(0, SeekOrigin.End)
							Else
								新建哈希表(哈希表路径)
							End If
						End Sub)
	End Sub
	Async Function 重置哈希表() As Task
		Await 哈希任务
		哈希流.Close()
		Dim 哈希表路径 As String = Path.Combine(设置项.下载保存路径, 文件名)
		File.Delete(哈希表路径)
		哈希任务 = 新建哈希表(哈希表路径)
	End Function
	Private Class 哈希比较
		Implements IEqualityComparer(Of Byte())

		Public Overloads Function Equals(x() As Byte, y() As Byte) As Boolean Implements IEqualityComparer(Of Byte()).Equals
			Return x.SequenceEqual(y)
		End Function

		Public Overloads Function GetHashCode(<DisallowNull> obj() As Byte) As Integer Implements IEqualityComparer(Of Byte()).GetHashCode
			Return obj.GetHashCode
		End Function
	End Class
	ReadOnly 哈希比较器 As New 哈希比较
	Async Function 尝试入库(新图 As Byte(), 文件名 As String) As Task(Of Boolean)
		Dim 新图哈希 As Byte() = 计算哈希(新图)
		Await 哈希任务
		If 检查库存(新图哈希) Then
			Return False
		Else
			哈希表.Add(新图哈希)
			哈希流.Write(新图哈希)
			哈希流.Flush()
			Dim 写出流 As FileStream = File.OpenWrite(Path.Combine(设置项.下载保存路径, 文件名))
			写出流.Write(新图)
			写出流.Close()
			Return True
		End If
	End Function
	Sub 保存哈希()
		哈希流.Close()
	End Sub
	Function 计算哈希(字节串 As Byte()) As Byte()
		Static SHA256计算器 As SHA256 = SHA256.Create
		Return SHA256计算器.ComputeHash(字节串)
	End Function
	Function 检查库存(哈希 As Byte()) As Boolean
		Return 哈希表.Contains(哈希, 哈希比较器)
	End Function
End Module
