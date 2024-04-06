Imports System.Data.SqlClient

Module Module1
    Function AgeCalculator(datetimepicker)
        Dim Age As Integer = Date.Today.Year - datetimepicker.Value.Date.Year
        Return Age
    End Function


    Sub ValidNum(ByVal textbox1 As TextBox)
        Dim text As String = textbox1.Text

        ' Check if the length of the text exceeds 10 characters
        If text.Length > 10 Then
            ' If it does, restrict further input by removing the last character
            textbox1.Text = text.Substring(0, 10)
            ' Display a message to notify the user
            MessageBox.Show("Mobile number cannot exceed 10 characters.")
        End If
    End Sub

    Public Function CHECKeMAIL(ByVal con As SqlConnection, email As String, table As String, Clm As String) As Boolean
        con.Open()

        ' Check if the generated StaffCode already exists in the table
        Dim query As String = "SELECT COUNT(*) FROM " & table & " WHERE " & Clm & " = @Email"
        Dim cmd As New SqlCommand(query, con)
        cmd.Parameters.AddWithValue("@Email", email)
        Dim count As Integer = Convert.ToInt32(cmd.ExecuteScalar())

        con.Close()

        If count = 0 Then
            Return False ' 0 or False for email not found
        Else
            Return True ' 1 or True for email found
        End If
    End Function

    Public Sub Populatedvg(con As SqlConnection, tablename As String, datagrid As DataGridView)
        ' Function to generate a Temporary Table from the Database into a DataGridView
        datagrid.Columns.Clear()
        con.Open()
        Dim sql = "SELECT * FROM " & tablename ' Remove the single quotes
        Dim adapter As SqlDataAdapter
        adapter = New SqlDataAdapter(sql, con)
        Dim builder As SqlCommandBuilder
        builder = New SqlCommandBuilder(adapter) ' This line should work correctly
        Dim ds As DataSet
        ds = New DataSet
        adapter.Fill(ds)
        datagrid.DataSource = ds.Tables(0)
        con.Close()
    End Sub

    Function GeneratePassword(TextBox1 As TextBox) As String
        If String.IsNullOrEmpty(TextBox1.Text) Then
            Return "@123"
        Else
            Dim cleanedText = TextBox1.Text.Replace(" ", "")
            Return cleanedText & "@123"
        End If
    End Function
    Sub AutoCompleteSearchBoxForTextBoxesTypeINt(con As SqlConnection, textbox1 As TextBox, ColumnName As String, TableName As String, columnNameIndex As Integer)
        con.Open()
        Dim Query As String = "SELECT " & ColumnName & " FROM " & TableName
        Dim Cmd As New SqlCommand(Query, con)
        Dim reader As SqlDataReader
        reader = Cmd.ExecuteReader
        Dim ElementsToSuggest As AutoCompleteStringCollection = New AutoCompleteStringCollection()

        While reader.Read
            ' Convert the Int32 column value to a string using ToString()
            ElementsToSuggest.Add(reader.GetInt32(columnNameIndex).ToString())
        End While

        textbox1.AutoCompleteCustomSource = ElementsToSuggest
        con.Close()
    End Sub
    Sub AutoCompleteSearchBoxForTextBoxesTypeString(con As SqlConnection, textbox1 As TextBox, ColumnName As String, TableName As String, columnNameIndex As Integer)
        con.Open()
        Dim Query As String = "SELECT " & ColumnName & " FROM " & TableName
        Dim Cmd As New SqlCommand(Query, con)
        Dim reader As SqlDataReader
        reader = Cmd.ExecuteReader
        Dim ElementsToSuggest As AutoCompleteStringCollection = New AutoCompleteStringCollection()

        While reader.Read
            ElementsToSuggest.Add(reader.GetString(columnNameIndex)) ' Use GetString to retrieve the column's value as a string

        End While
        textbox1.AutoCompleteCustomSource = ElementsToSuggest
        con.Close()
    End Sub


    Sub FillcomboBox(con As SqlConnection, cmbx As ComboBox, tblName As String, ColumnName As String)
        ' Open the connection
        con.Open()

        ' Create a SQL command to select all data from the specified table
        Dim cmd As New SqlCommand("SELECT * FROM " & tblName, con)

        ' Create a data adapter and a DataTable
        Dim adapter As New SqlDataAdapter(cmd)
        Dim Tbl As New DataTable

        ' Fill the DataTable with data from the database
        adapter.Fill(Tbl)

        ' Set the ComboBox's data source and member bindings
        cmbx.DataSource = Tbl
        cmbx.DisplayMember = ColumnName
        cmbx.ValueMember = ColumnName

        ' Close the connection
        con.Close()
    End Sub


    Sub FillSelectedColumnIncomboBox(con As SqlConnection, cmbx As ComboBox, tblName As String, ColumnName As String, ColumnName2 As String, ClmData As String)
        Try
            ' Open the connection
            con.Open()

            ' Create a SQL command with parameters to select data from the specified table based on the condition
            Dim query As String = "SELECT * FROM " & tblName & " WHERE " & ColumnName2 & " = @ClmData"
            Dim cmd As New SqlCommand(query, con)
            cmd.Parameters.AddWithValue("@ClmData", ClmData)

            ' Create a data adapter and a DataTable
            Dim adapter As New SqlDataAdapter(cmd)
            Dim Tbl As New DataTable

            ' Fill the DataTable with data from the database
            adapter.Fill(Tbl)

            ' Set the ComboBox's data source and member bindings
            cmbx.DataSource = Tbl
            cmbx.DisplayMember = ColumnName
            cmbx.ValueMember = ColumnName

        Catch ex As Exception
            ' Handle exceptions (display or log error message)
            MessageBox.Show("Error: " & ex.Message)

        Finally
            ' Close the connection
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
        End Try
    End Sub




End Module
