Imports System.Data.SqlClient
Imports System.Windows.Forms.VisualStyles.VisualStyleElement

Public Class DoctorForm
    Dim connectionString As String = "Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\user\source\repos\DiagnosticCenter\DiagnosticCenter\DiagnosticCenterDatabase.mdf;Integrated Security=True"
    Dim con As New SqlConnection(connectionString)
    Dim StaffId As String
    Private Sub DoctorForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Populatedvg(con, "AppointMentData", DataGridView6)
        Populatedvg(con, "ReportData", DataGridView4)
        Populatedvg(con, "TestData", DataGridView3)
        FillPersonalDetailsInFeilds()
        Populatedvg(con, "PaymentData", DataGridView2)
        StaffId = TextBox1.Text

    End Sub
    Private Function IsAllAttntFieldsFilled() As Boolean
        ' Check if all required fields are filled
        Return Not (
            String.IsNullOrWhiteSpace(Txt_AppntDoctorID.Text) OrElse
            String.IsNullOrWhiteSpace(Txt_AppntDoctorRoomNo.Text) OrElse
            String.IsNullOrWhiteSpace(Txt_AppntPtnID.Text) OrElse
            String.IsNullOrWhiteSpace(Txt_AppntPtnName.Text) OrElse
            String.IsNullOrWhiteSpace(Txt_AppntPtnPhnNumber.Text) OrElse
            String.IsNullOrWhiteSpace(Txt_AppntTestCost.Text) OrElse
            String.IsNullOrWhiteSpace(Txt_AppntTestID.Text) OrElse
            String.IsNullOrWhiteSpace(Txt_AppntTestName.Text) OrElse
            String.IsNullOrWhiteSpace(Txt_AppntDoctorName.Text)
           )
        ' Optionally, you can remove additional conditions (CheckBoxInternetBanking, CheckBoxMobilrBanking, CheckBoxChequebook, CheckBoxemailAlerts, CheckBoxestatement)
    End Function

    Private Sub ClearAllAppntControls()
        ' Clear all input controls
        Dim controlsToClear() As Control = {Txt_AppntDoctorName, Txt_AppntTestName, Txt_AppntDoctorID, Txt_AppntDoctorRoomNo, Txt_AppntPtnID, Txt_AppntPtnName, Txt_AppntPtnPhnNumber, Txt_AppntTestCost, Txt_AppntTestID}
        For Each control As Control In controlsToClear
            control.Text = String.Empty
        Next
    End Sub
    Private Sub Guna2GradientButton22_Click(sender As Object, e As EventArgs) Handles Guna2GradientButton22.Click

        Try
            If con.State = ConnectionState.Open Then
                con.Close()
            End If

            con.Open()
            Dim query As String = "INSERT INTO ReportData (PatientName, PatientID, PatientPhoneNumber,TestName,TestID,TestCost,DoctorName,DoctorID,DoctotRoomNo,ReportDate) " &
                        "VALUES (@PatientName, @PatientID, @PatientPhoneNumber,@TestName,@TestID,@TestCost,@DoctorName,@DoctorID,@DoctotRoomNo,@ReportDate)"
            Using cmd As New SqlCommand(query, con)
                ' Set parameter values...'
                cmd.Parameters.AddWithValue("@PatientName", If(Not String.IsNullOrEmpty(Txt_AppntPtnName.Text), Txt_AppntPtnName.Text, DBNull.Value))
                cmd.Parameters.AddWithValue("@PatientID", If(Not String.IsNullOrEmpty(Txt_AppntPtnID.Text), Txt_AppntPtnID.Text, DBNull.Value))
                cmd.Parameters.AddWithValue("@PatientPhoneNumber", If(Not String.IsNullOrEmpty(Txt_AppntPtnPhnNumber.Text), Txt_AppntPtnPhnNumber.Text, DBNull.Value))
                cmd.Parameters.AddWithValue("@TestName", If(Not String.IsNullOrEmpty(Txt_AppntTestName.Text), Txt_AppntTestName.Text, DBNull.Value))
                cmd.Parameters.AddWithValue("@TestID", If(Not String.IsNullOrEmpty(Txt_AppntTestID.Text), Txt_AppntTestID.Text, DBNull.Value))
                cmd.Parameters.AddWithValue("@TestCost", If(Not String.IsNullOrEmpty(Txt_AppntTestCost.Text), Txt_AppntTestCost.Text, DBNull.Value))
                cmd.Parameters.AddWithValue("@DoctorName", If(Not String.IsNullOrEmpty(Txt_AppntDoctorName.Text), Txt_AppntDoctorName.Text, DBNull.Value))
                cmd.Parameters.AddWithValue("@DoctorID", If(Not String.IsNullOrEmpty(Txt_AppntDoctorID.Text), Txt_AppntDoctorID.Text, DBNull.Value))
                cmd.Parameters.AddWithValue("@DoctotRoomNo", If(Not String.IsNullOrEmpty(Txt_AppntDoctorRoomNo.Text), Txt_AppntDoctorRoomNo.Text, DBNull.Value))
                cmd.Parameters.AddWithValue("@ReportDate", DTP_ReportDate.Value.Date)
                ' Execute the query...'
                cmd.ExecuteNonQuery()

                ' Display success message...'
                MsgBox("Patient : " & Txt_AppntPtnName.Text & " Appointment Data has been added Successfully ")
                Dim deleteQuery As String = "DELETE FROM AppointMentData WHERE AppointMentID = @AppointMentID"
                Dim cmdDelete As New SqlCommand(deleteQuery, con)
                cmdDelete.Parameters.AddWithValue("@AppointMentID", Txt_AppntAppntID.Text)
                cmdDelete.ExecuteNonQuery()
                ' Clear controls...
                ClearAllAppntControls()
            End Using
        Catch ex As Exception
            ' Display error message...
            MsgBox("Error: " & ex.Message)
        Finally
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
        End Try
        Populatedvg(con, "AppointMentData", DataGridView6)
    End Sub


    Private Sub Guna2GradientButton2_Click(sender As Object, e As EventArgs) Handles Guna2GradientButton2.Click
        If Not String.IsNullOrEmpty(Txt_AppntAppntID.Text) Then
            con.Open()
            Try
                ' Check if the staff ID exists in the database
                Dim query As String = "SELECT * FROM AppointMentData WHERE AppointMentID = @AppointMentID"
                Using cmd As New SqlCommand(query, con)
                    cmd.Parameters.AddWithValue("@AppointMentID", Txt_AppntAppntID.Text)

                    ' Execute the query and check if any rows were returned
                    Dim reader As SqlDataReader = cmd.ExecuteReader()
                    If reader.HasRows Then
                        reader.Read()
                        ' Populate the fields with retrieved data
                        Txt_AppntPtnName.Text = reader(1) ' Name
                        Txt_AppntPtnID.Text = reader(2) ' ptnid
                        Txt_AppntPtnPhnNumber.Text = reader(3) ' Phn num
                        Txt_AppntTestName.Text = reader(4) ' Test Name
                        Txt_AppntTestID.Text = reader(5) ' test ID
                        Txt_AppntTestCost.Text = reader(6) ' test Cost
                        Txt_AppntDoctorName.Text = reader(7) '  Doctor name
                        Txt_AppntDoctorID.Text = reader(8) ' Doctor ID
                        Txt_AppntDoctorRoomNo.Text = reader(9) ' Doctor room
                    End If
                End Using
            Catch ex As Exception
                ' Handle the exception (e.g., display an error message)
                MessageBox.Show("An error occurred: " & ex.Message)
            Finally
                con.Close()
            End Try

        End If
    End Sub

    Private Sub Guna2GradientButton21_Click(sender As Object, e As EventArgs) Handles Guna2GradientButton21.Click
        ClearAllAppntControls()

    End Sub

    Private Sub Guna2GradientButton1_Click(sender As Object, e As EventArgs) Handles Guna2GradientButton1.Click
        Try
            If con.State = ConnectionState.Open Then
                con.Close()
            End If

            If Not String.IsNullOrEmpty(TextBox36.Text) Then
                DataGridView6.Columns.Clear()
                con.Open()

                Dim sql As String = ""
                Dim Colmntype As String = "AppointMentID"


                Dim tablename As String = "AppointMentData"

                sql = "SELECT * FROM " & tablename & " WHERE " & Colmntype & " = @Condition"

                Dim adapter As SqlDataAdapter
                adapter = New SqlDataAdapter(sql, con)

                adapter.SelectCommand.Parameters.AddWithValue("@Condition", TextBox36.Text)
                Dim builder As SqlCommandBuilder
                builder = New SqlCommandBuilder(adapter)

                Dim ds As DataSet
                ds = New DataSet
                adapter.Fill(ds)

                DataGridView6.DataSource = ds.Tables(0)
            Else
                MsgBox("Please Enter the Staff ID To Search")
            End If
        Catch ex As Exception
            MessageBox.Show("An error occurred: " & ex.Message)
        Finally
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
        End Try
    End Sub

    Private Sub TextBox36_TextChanged(sender As Object, e As EventArgs) Handles TextBox36.TextChanged
        If TextBox36.Text = "" Then
            Populatedvg(con, "AppointMentData", DataGridView6)
        End If
    End Sub

    Private Sub Guna2GradientButton13_Click(sender As Object, e As EventArgs) Handles Guna2GradientButton13.Click
        Try
            ' Close the connection if it's already open
            If con.State = ConnectionState.Open Then
                con.Close()
            End If

            ' Clear previous data in DataGridView3
            DataGridView4.DataSource = Nothing
            DataGridView4.Columns.Clear()

            ' Check if TextBox38 is not empty
            If Not String.IsNullOrEmpty(TextBox16.Text) Then
                ' Open the connection
                con.Open()

                Dim sql As String = ""
                Dim Colmntype As String = ""
                Dim tablename As String = "ReportData"


                ' Determine the column based on ComboBox selection
                If ComboBox5.SelectedIndex = 0 Then
                    Colmntype = "PatientName"
                ElseIf ComboBox5.SelectedIndex = 1 Then
                    Colmntype = "DoctorName"
                ElseIf ComboBox5.SelectedIndex = 2 Then
                    Colmntype = "TestName"
                End If

                ' Create the SQL query with a parameterized query
                sql = "SELECT * FROM " & tablename & " WHERE " & Colmntype & " = @Condition"

                ' Create SQL data adapter
                Dim adapter As SqlDataAdapter = New SqlDataAdapter(sql, con)
                adapter.SelectCommand.Parameters.AddWithValue("@Condition", TextBox16.Text)

                ' Create DataSet to hold the fetched data
                Dim ds As DataSet = New DataSet()
                adapter.Fill(ds)

                ' Bind the data to DataGridView3
                DataGridView4.DataSource = ds.Tables(0)
            Else
                ' Show a message if TextBox38 is empty
                MsgBox("Please Enter the Detail in Search box To Search")
            End If
        Catch ex As Exception
            ' Display error message if an exception occurs
            MessageBox.Show("An error occurred: " & ex.Message)
        Finally
            ' Close the connection if it's open
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
        End Try
    End Sub


    Private Sub ComboBox5_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox5.SelectedIndexChanged
        Try
            con.Open()

            Dim Colmntype As String = ""
            Dim tablename As String = "ReportData"

            ' Determine the column based on ComboBox selection
            If ComboBox5.SelectedIndex = 0 Then
                Colmntype = "PatientName"
            ElseIf ComboBox5.SelectedIndex = 1 Then
                Colmntype = "DoctorName"
            ElseIf ComboBox5.SelectedIndex = 2 Then
                Colmntype = "TestName"
            End If

            Dim Query As String = "SELECT " & Colmntype & " FROM " & tablename
            Dim Cmd As New SqlCommand(Query, con)
            Dim reader As SqlDataReader = Cmd.ExecuteReader()
            Dim ElementsToSuggest As AutoCompleteStringCollection = New AutoCompleteStringCollection()

            While reader.Read
                ' Check if the column index is valid
                If Not reader.IsDBNull(0) Then
                    ElementsToSuggest.Add(reader.GetString(0)) ' Use GetString to retrieve the column's value as a string
                End If
            End While

            TextBox16.AutoCompleteCustomSource = ElementsToSuggest
        Catch ex As Exception
            ' Handle exceptions (display or log error message)
            MessageBox.Show("Error: " & ex.Message)
        Finally
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
        End Try
    End Sub

    Private Sub TextBox16_TextChanged(sender As Object, e As EventArgs) Handles TextBox16.TextChanged
        If Not TextBox16.Text = "" Then
            Populatedvg(con, "ReportData", DataGridView4)

        End If
    End Sub
    Private Function IsAllTestFieldsFilled() As Boolean
        ' Check if all required fields are filled
        Return Not (
            String.IsNullOrWhiteSpace(Txt_TestDiscriptons.Text) OrElse
            String.IsNullOrWhiteSpace(Txt_TestName.Text) OrElse
            String.IsNullOrWhiteSpace(Txt_TestName.Text) OrElse
            Txt_TestType.SelectedIndex = -1)
        ' Optionally, you can remove additional conditions (CheckBoxInternetBanking, CheckBoxMobilrBanking, CheckBoxChequebook, CheckBoxemailAlerts, CheckBoxestatement)
    End Function

    Private Sub ClearAllTestControls()
        ' Clear all input controls
        Dim controlsToClear() As Control = {Txt_TestDiscriptons, Txt_TestName, Txt_TestCharges, Txt_TestID}
        For Each control As Control In controlsToClear
            control.Text = String.Empty
        Next
        Txt_TestType.SelectedIndex = -1
    End Sub
    Private Sub Guna2GradientButton25_Click(sender As Object, e As EventArgs) Handles Guna2GradientButton25.Click
        If Not String.IsNullOrEmpty(Txt_TestID.Text) Then

            con.Open()
            Try
                ' Check if the staff ID exists in the database
                Dim query As String = "SELECT * FROM TestData WHERE TestID = @TestID"
                Using cmd As New SqlCommand(query, con)
                    cmd.Parameters.AddWithValue("@TestID", Txt_TestID.Text)

                    ' Execute the query and check if any rows were returned
                    Dim reader As SqlDataReader = cmd.ExecuteReader()
                    If reader.HasRows Then
                        reader.Read()
                        ' Populate the fields with retrieved data
                        Txt_TestName.Text = reader(1) ' Name
                        Txt_TestType.Text = reader(2) ' TestType
                        Txt_TestCharges.Text = reader(3) ' Charges
                        Txt_TestDiscriptons.Text = reader(4) ' Discription
                    Else
                        MessageBox.Show("Test ID number not found.")
                    End If
                End Using
            Catch ex As Exception
                ' Handle the exception (e.g., display an error message)
                MessageBox.Show("An error occurred: " & ex.Message)
            Finally
                con.Close()
            End Try
        Else
            MessageBox.Show("Please enter a Test ID number.")
        End If

        Populatedvg(con, "TestData", DataGridView3)
    End Sub

    Private Sub Guna2GradientButton12_Click(sender As Object, e As EventArgs) Handles Guna2GradientButton12.Click
        If Not IsAllTestFieldsFilled() Then
            MsgBox("Please fill in all the Test details.")
            Exit Sub
        End If
        If CHECKeMAIL(con, Txt_TestName.Text, "TestData", "TestName") = True Then
            MsgBox("This Test Already Exist..")
            Exit Sub
        End If
        Try
            If con.State = ConnectionState.Open Then
                con.Close()
            End If

            con.Open()
            Dim query As String = "INSERT INTO TestData (TestName, TestType, Charges,Discriptions) " &
                        "VALUES (@TestName, @TestType, @Charges,@Discriptions)"
            Using cmd As New SqlCommand(query, con)
                ' Set parameter values...'
                cmd.Parameters.AddWithValue("@TestName", If(Not String.IsNullOrEmpty(Txt_TestName.Text), Txt_TestName.Text, DBNull.Value))
                cmd.Parameters.AddWithValue("@TestType", If(Txt_TestType.SelectedIndex <> -1, Txt_TestType.SelectedItem.ToString(), DBNull.Value))
                cmd.Parameters.AddWithValue("@Charges", If(Not String.IsNullOrEmpty(Txt_TestCharges.Text), Txt_TestCharges.Text, DBNull.Value))
                cmd.Parameters.AddWithValue("@Discriptions", If(Not String.IsNullOrEmpty(Txt_TestDiscriptons.Text), Txt_TestDiscriptons.Text, DBNull.Value))
                ' Execute the query...'
                cmd.ExecuteNonQuery()

                ' Display success message...'
                MsgBox("New Lab Test : " & Txt_TestName.Text & " Data has been added Successfully ")

                ' Clear controls...
                ClearAllTestControls()
            End Using
        Catch ex As Exception
            ' Display error message...
            MsgBox("Error: " & ex.Message)
        Finally
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
        End Try
        Populatedvg(con, "TestData", DataGridView3)
    End Sub

    Private Sub Guna2GradientButton11_Click(sender As Object, e As EventArgs) Handles Guna2GradientButton11.Click
        If Not String.IsNullOrEmpty(Txt_TestID.Text) Then
            If Not IsAllTestFieldsFilled() Then
                MsgBox("Please fill in all the Test details.")
                Exit Sub
            End If

            Try
                If con.State = ConnectionState.Open Then
                    con.Close()
                End If

                con.Open()
                Dim query As String = "UPDATE TestData SET TestName = @TestName, TestType = @TestType, Charges = @Charges, Discriptions = @Discriptions WHERE TestID = @TestID"
                Using cmd As New SqlCommand(query, con)
                    ' Set parameter values...'
                    cmd.Parameters.AddWithValue("@TestName", If(Not String.IsNullOrEmpty(Txt_TestName.Text), Txt_TestName.Text, DBNull.Value))
                    cmd.Parameters.AddWithValue("@TestType", If(Txt_TestType.SelectedIndex <> -1, Txt_TestType.SelectedItem.ToString(), DBNull.Value))
                    cmd.Parameters.AddWithValue("@Charges", If(Not String.IsNullOrEmpty(Txt_TestCharges.Text), Txt_TestCharges.Text, DBNull.Value))
                    cmd.Parameters.AddWithValue("@Discriptions", If(Not String.IsNullOrEmpty(Txt_TestDiscriptons.Text), Txt_TestDiscriptons.Text, DBNull.Value))
                    cmd.Parameters.AddWithValue("@TestID", If(Not String.IsNullOrEmpty(Txt_TestID.Text), Txt_TestID.Text, DBNull.Value))
                    ' Execute the query...'
                    Dim rowsAffected As Integer = cmd.ExecuteNonQuery()

                    If rowsAffected > 0 Then
                        ' Display success message if update was successful...'
                        MsgBox("Lab Test data for " & Txt_TestName.Text & " has been updated successfully.")
                    Else
                        ' Notify if no record was updated (TestName might not have been found)...
                        MsgBox("No records found for Test: " & Txt_TestName.Text & ". Update failed.")
                    End If

                    ' Clear controls...
                    ClearAllTestControls()
                End Using
            Catch ex As Exception
                ' Display error message...
                MsgBox("Error: " & ex.Message)
            Finally
                If con.State = ConnectionState.Open Then
                    con.Close()
                End If
            End Try
        Else
            MessageBox.Show("Please Enter the Test ID")
        End If

        Populatedvg(con, "TestData", DataGridView3)
    End Sub

    Private Sub Guna2GradientButton10_Click(sender As Object, e As EventArgs) Handles Guna2GradientButton10.Click
        ClearAllTestControls()
    End Sub

    Private Sub Guna2GradientButton9_Click(sender As Object, e As EventArgs) Handles Guna2GradientButton9.Click
        Try
            If con.State = ConnectionState.Open Then
                con.Close()
            End If

            If Not String.IsNullOrEmpty(Txt_TestNameSearch.Text) Then
                DataGridView3.Columns.Clear()
                con.Open()
                Dim sql As String = ""
                Dim Colmntype As String = "TestName"
                Dim tablename As String = "TestData"

                sql = "SELECT * FROM " & tablename & " WHERE " & Colmntype & " = @Condition"

                Dim adapter As SqlDataAdapter
                adapter = New SqlDataAdapter(sql, con)

                adapter.SelectCommand.Parameters.AddWithValue("@Condition", Txt_TestNameSearch.Text)
                Dim builder As SqlCommandBuilder
                builder = New SqlCommandBuilder(adapter)

                Dim ds As DataSet
                ds = New DataSet
                adapter.Fill(ds)

                DataGridView3.DataSource = ds.Tables(0)
            Else
                MsgBox("Please Enter the Lab Test ID To Search")
            End If
        Catch ex As Exception
            MessageBox.Show("An error occurred: " & ex.Message)
        Finally
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
        End Try
    End Sub

    Private Sub Txt_TestNameSearch_TextChanged(sender As Object, e As EventArgs) Handles Txt_TestNameSearch.TextChanged
        If Txt_TestNameSearch.Text = "" Then
            Populatedvg(con, "TestData", DataGridView3)
        End If
    End Sub

    Private Sub CheckBox1_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox1.CheckedChanged
        If CheckBox1.Checked Then
            Txt_TestID.Enabled = True
        Else
            Txt_TestID.Enabled = False
        End If
    End Sub

    Private Sub Guna2GradientButton17_Click(sender As Object, e As EventArgs) Handles Guna2GradientButton17.Click
        If Not String.IsNullOrEmpty(TextBox1.Text) Then
            con.Open()
            Try
                ' Check if the patient ID exists in the database
                Dim query = "SELECT * FROM StaffData WHERE StaffId = @StaffId"
                Using cmd As New SqlCommand(query, con)
                    cmd.Parameters.AddWithValue("@StaffId", TextBox1.Text)

                    ' Execute the query and check if any rows were returned
                    Dim reader As SqlDataReader = cmd.ExecuteReader()
                    If reader.HasRows Then
                        reader.Read()
                        Txt_prsnlStffName.Text = reader(1) ' Name
                        DTP_PrsnlDOBstff.Value = reader(2) ' Date of Birth
                        Txt_prsnlStffGender.Text = reader(4) ' gender
                        Txt_prsnlStffPhone.Text = reader(5) ' PhnNo
                        Txt_prsnlStffEmail.Text = reader(6) ' Email
                        Txt_prsnlStffPass.Text = reader(7) ' Password
                        DPTjoiningPrsnlStaff.Value = reader(8) ' DOJ
                        Txt_prsnlStffdesignation.Text = reader(9) ' Address
                        Txt_prsnlStffAddress.Text = reader(11) ' Address
                        Txt_prsnlStffRoomNo.Text = reader(12) ' Address
                        Txt_StaffsplswdIn.Text = reader(10)
                    End If
                End Using
            Catch ex As Exception
                ' Handle the exception (e.g., display an error message)
                MessageBox.Show("An error occurred: " & ex.Message)
            Finally
                con.Close()
            End Try
        End If
    End Sub



    Private Sub FillPersonalDetailsInFeilds()
        If Not String.IsNullOrEmpty(TextBox1.Text) Then
            con.Open()
            Try
                ' Check if the patient ID exists in the database
                Dim query = "SELECT * FROM StaffData WHERE StaffId = @StaffId"
                Using cmd As New SqlCommand(query, con)
                    cmd.Parameters.AddWithValue("@StaffId", TextBox1.Text)

                    ' Execute the query and check if any rows were returned
                    Dim reader As SqlDataReader = cmd.ExecuteReader()
                    If reader.HasRows Then
                        reader.Read()
                        Txt_prsnlStffName.Text = reader(1) ' Name
                        DTP_PrsnlDOBstff.Value = reader(2) ' Date of Birth
                        Txt_prsnlStffGender.Text = reader(4) ' gender
                        Txt_prsnlStffPhone.Text = reader(5) ' PhnNo
                        Txt_prsnlStffEmail.Text = reader(6) ' Email
                        Txt_prsnlStffPass.Text = reader(7) ' Password
                        DPTjoiningPrsnlStaff.Value = reader(8) ' DOJ
                        Txt_prsnlStffdesignation.Text = reader(9) ' Address
                        Txt_prsnlStffAddress.Text = reader(11) ' Address
                        Txt_prsnlStffRoomNo.Text = reader(12) ' Address
                        Txt_StaffsplswdIn.Text = reader(10)
                    End If
                End Using
            Catch ex As Exception
                ' Handle the exception (e.g., display an error message)
                MessageBox.Show("An error occurred: " & ex.Message)
            Finally
                con.Close()
            End Try
        End If
    End Sub

    Private Sub Guna2GradientButton19_Click(sender As Object, e As EventArgs) Handles Guna2GradientButton19.Click


        Try
            Dim age As Integer = AgeCalculator(DTP_PrsnlDOBstff)

            If con.State = ConnectionState.Open Then
                con.Close()
            End If

            con.Open()
            Dim query As String = "UPDATE StaffData SET Name = @Name, DOB = @DOB, Age = @Age, Gender = @Gender, PhoneNo = @PhoneNo, Email = @Email ," &
                          "Password=@Password,JoiningDate = @JoiningDate, Desgnation = @Desgnation, Address = @Address, RoomNo = @RoomNo " &
                          "WHERE StaffID = @StaffID" ' Assuming StaffID is the primary key to identify staff members

            Using cmd As New SqlCommand(query, con)
                ' Set parameter values...'
                cmd.Parameters.AddWithValue("@Name", If(Not String.IsNullOrEmpty(Txt_prsnlStffName.Text), Txt_prsnlStffName.Text, DBNull.Value))
                cmd.Parameters.AddWithValue("@DOB", DTP_PrsnlDOBstff.Value.Date)
                cmd.Parameters.AddWithValue("@Age", age)
                cmd.Parameters.AddWithValue("@Gender", If(Txt_prsnlStffGender.SelectedIndex <> -1, Txt_prsnlStffGender.SelectedItem.ToString(), DBNull.Value))
                cmd.Parameters.AddWithValue("@PhoneNo", If(Not String.IsNullOrEmpty(Txt_prsnlStffPhone.Text), Txt_prsnlStffPhone.Text, DBNull.Value))
                cmd.Parameters.AddWithValue("@Email", If(Not String.IsNullOrEmpty(Txt_prsnlStffEmail.Text), Txt_prsnlStffEmail.Text, DBNull.Value))
                cmd.Parameters.AddWithValue("@Password", If(Not String.IsNullOrEmpty(Txt_prsnlStffPass.Text), Txt_prsnlStffPass.Text, DBNull.Value))
                cmd.Parameters.AddWithValue("@JoiningDate", DTP_PrsnlDOBstff.Value.Date)
                cmd.Parameters.AddWithValue("@Desgnation", If(Txt_prsnlStffdesignation.SelectedIndex <> -1, Txt_prsnlStffdesignation.SelectedItem.ToString(), DBNull.Value))
                cmd.Parameters.AddWithValue("@Address", If(Not String.IsNullOrEmpty(Txt_prsnlStffAddress.Text), Txt_prsnlStffAddress.Text, DBNull.Value))
                cmd.Parameters.AddWithValue("@StaffID", StaffId) ' Replace staffID with the actual ID of the staff member you want to update
                cmd.Parameters.AddWithValue("@RoomNo", If(Not String.IsNullOrEmpty(Txt_prsnlStffRoomNo.Text), Txt_prsnlStffRoomNo.Text, DBNull.Value))

                ' Execute the query...'
                cmd.ExecuteNonQuery()

                ' Display success message...'
                MsgBox("Your Details has been updated successfully")

                ' Clear controls...
            End Using
        Catch ex As Exception
            ' Display error message...
            MsgBox("Error: " & ex.Message)
        Finally
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
        End Try
    End Sub

    Private Sub Guna2GradientButton5_Click(sender As Object, e As EventArgs) Handles Guna2GradientButton5.Click
        Try
            ' Close the connection if it's already open
            If con.State = ConnectionState.Open Then
                con.Close()
            End If

            ' Clear previous data in DataGridView3
            DataGridView2.DataSource = Nothing
            DataGridView2.Columns.Clear()

            ' Check if TextBox38 is not empty
            If Not String.IsNullOrEmpty(TextBox3.Text) Then
                ' Open the connection
                con.Open()

                Dim sql As String = ""
                Dim Colmntype As String = ""
                Dim tablename As String = "PaymentData"


                ' Determine the column based on ComboBox selection
                If ComboBox2.SelectedIndex = 0 Then
                    Colmntype = "PatientName"
                ElseIf ComboBox2.SelectedIndex = 1 Then
                    Colmntype = "DoctorName"
                ElseIf ComboBox2.SelectedIndex = 2 Then
                    Colmntype = "TestName"
                End If

                ' Create the SQL query with a parameterized query
                sql = "SELECT * FROM " & tablename & " WHERE " & Colmntype & " = @Condition"

                ' Create SQL data adapter
                Dim adapter As SqlDataAdapter = New SqlDataAdapter(sql, con)
                adapter.SelectCommand.Parameters.AddWithValue("@Condition", TextBox3.Text)

                ' Create DataSet to hold the fetched data
                Dim ds As DataSet = New DataSet()
                adapter.Fill(ds)

                ' Bind the data to DataGridView3
                DataGridView2.DataSource = ds.Tables(0)
            Else
                ' Show a message if TextBox38 is empty
                MsgBox("Please Enter the The Detail in Search box To Search")
            End If
        Catch ex As Exception
            ' Display error message if an exception occurs
            MessageBox.Show("An error occurred: " & ex.Message)
        Finally
            ' Close the connection if it's open
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
        End Try
    End Sub

    Private Sub TextBox3_TextChanged(sender As Object, e As EventArgs) Handles TextBox3.TextChanged
        If TextBox3.Text = "" Then
            Populatedvg(con, "PaymentData", DataGridView2)
        End If
    End Sub

    Private Sub ComboBox2_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox2.SelectedIndexChanged
        Try
            con.Open()

            Dim Colmntype As String = ""
            Dim tablename As String = "PaymentData"

            ' Determine the column based on ComboBox selection
            If ComboBox2.SelectedIndex = 0 Then
                Colmntype = "PatientName"
            ElseIf ComboBox2.SelectedIndex = 1 Then
                Colmntype = "DoctorName"
            ElseIf ComboBox2.SelectedIndex = 2 Then
                Colmntype = "TestName"
            End If

            Dim Query As String = "SELECT " & Colmntype & " FROM " & tablename
            Dim Cmd As New SqlCommand(Query, con)
            Dim reader As SqlDataReader = Cmd.ExecuteReader()
            Dim ElementsToSuggest As AutoCompleteStringCollection = New AutoCompleteStringCollection()

            While reader.Read
                ' Check if the column index is valid
                If Not reader.IsDBNull(0) Then
                    ElementsToSuggest.Add(reader.GetString(0)) ' Use GetString to retrieve the column's value as a string
                End If
            End While

            TextBox3.AutoCompleteCustomSource = ElementsToSuggest
        Catch ex As Exception
            ' Handle exceptions (display or log error message)
            MessageBox.Show("Error: " & ex.Message)
        Finally
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
        End Try
    End Sub

    Private Sub Guna2CircleButton2_Click(sender As Object, e As EventArgs) Handles Guna2CircleButton2.Click
        Populatedvg(con, "AppointMentData", DataGridView6)
        Populatedvg(con, "ReportData", DataGridView4)
        Populatedvg(con, "TestData", DataGridView3)
        FillPersonalDetailsInFeilds()
        Populatedvg(con, "PaymentData", DataGridView2)
        StaffId = TextBox1.Text
    End Sub

    Private Sub Guna2GradientButton3_Click(sender As Object, e As EventArgs) Handles Guna2GradientButton3.Click
        LoginForm.Show()
        Me.Hide()
    End Sub

    Private Sub Txt_AppntPtnPhnNumber_TextChanged(sender As Object, e As EventArgs) Handles Txt_AppntPtnPhnNumber.TextChanged
        'ValidNum(Txt_AppntPtnPhnNumber)
    End Sub

    Private Sub Txt_AppntPtnPhnNumber_KeyPress(sender As Object, e As KeyPressEventArgs) Handles Txt_AppntPtnPhnNumber.KeyPress
        ' Check if the entered key is a number or a control key (like backspace)
        If Not Char.IsControl(e.KeyChar) AndAlso Not Char.IsDigit(e.KeyChar) Then
            ' If it's not a number or control key, suppress the key press
            e.Handled = True
        End If
    End Sub
    Private Sub Txt_AppntPtnPhnNumber_Validating(sender As Object, e As System.ComponentModel.CancelEventArgs) Handles Txt_AppntPtnPhnNumber.Validating
        ' Check if the entered value in Txt_PtnPhnNo is a valid phone number
        Dim phoneNumber As String = Txt_AppntPtnPhnNumber.Text

        ' Perform validation - example: Check if it contains only digits and has a valid length
        If Not System.Text.RegularExpressions.Regex.IsMatch(phoneNumber, "^[0-9]{10}$") Then
            ' If the input is invalid, show an error message
            MessageBox.Show("Please enter a valid 10-digit phone number.")
            ' Set CancelEventArgs.Cancel to True to prevent focus change
            e.Cancel = True
        Else
            ' If the input is valid, reset the Cancel property to allow focus change
            e.Cancel = False
        End If
    End Sub

End Class