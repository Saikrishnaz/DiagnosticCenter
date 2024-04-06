Imports System.Data.SqlClient
' to print a report lets import a header file
Imports System.Drawing.Printing
Imports System.Threading
Imports System.Web.Management
Imports System.Web.UI.WebControls
Imports System.Windows.Forms.VisualStyles.VisualStyleElement

Public Class Form1
    Dim EmpID As String
    Dim connectionString As String = "Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\user\source\repos\DiagnosticCenter\DiagnosticCenter\DiagnosticCenterDatabase.mdf;Integrated Security=True"
    Dim con As New SqlConnection(connectionString)
    Dim globaltockenNo As Integer = 0
    Dim PhoneNumberPymnt As String = ""
    Dim DoctorNamePymnt As String = ""
    Dim TestIDPymnt As Integer = 0
    Dim DoctorRoomNoPymnt As Integer = 0
    Dim DoctorIDPymnt As Integer = 0
    Dim TestCostPymnt As Decimal = 0

    Dim StaffId As String = ""


    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        FillPersonalDetailsInFeilds()
        DTP_DOBPtn.MaxDate = Today.Date
        DTP_StffDob.MaxDate = Today.Date
        Populatedvg(con, "CustomerData", DataGridView1)
        Populatedvg(con, "StaffData", DataGridView2)
        Populatedvg(con, "TestData", DataGridView3)
        Populatedvg(con, "AppointMentData", DataGridView6)
        Populatedvg(con, "ReportData", DataGridView4)
        Populatedvg(con, "ReportData", DataGridView5)
        FillcomboBox(con, Txt_AppntTestName, "TestData", "TestName")
        FillSelectedColumnIncomboBox(con, Txt_AppntDoctorName, "StaffData", "Name", "Desgnation", "Doctor")
        AutoCompleteSearchBoxForTextBoxesTypeINt(con, Txt_AppntPtnName, "PatientID", "CustomerData", 0)
        AutoCompleteSearchBoxForTextBoxesTypeString(con, Txt_TestNameSearch, "TestName", "TestData", 0)
        AutoCompleteSearchBoxForTextBoxesTypeString(con, Txt_TestNameSearch, "TestName", "TestData", 0)
        AutoCompleteSearchBoxForTextBoxesTypeINt(con, Txt_Ptnid, "PatientID", "CustomerData", 0)
        AutoCompleteSearchBoxForTextBoxesTypeINt(con, Txt_StaffId, "StaffId", "StaffData", 0)
        AutoCompleteSearchBoxForTextBoxesTypeINt(con, TextBox36, "AppointMentID", "AppointMentData", 0)
        StaffId = TextBox1.Text

    End Sub
    Private Function IsAllPTNFieldsFilled() As Boolean
        ' Check if all required fields are filled
        Return Not (
            String.IsNullOrWhiteSpace(Txt_PtnName.Text) OrElse
            String.IsNullOrWhiteSpace(Txt_PtnAddress.Text) OrElse
            String.IsNullOrWhiteSpace(Txt_PtnPhnNo.Text) OrElse
            Txt_PtnGender.SelectedIndex = -1 OrElse
            Txt_PtnBloodGrp.SelectedIndex = -1)
        ' Optionally, you can remove additional conditions (CheckBoxInternetBanking, CheckBoxMobilrBanking, CheckBoxChequebook, CheckBoxemailAlerts, CheckBoxestatement)
    End Function
    Private Sub Guna2GradientButton3_Click(sender As Object, e As EventArgs) Handles Guna2GradientButton3.Click
        If Not IsAllPTNFieldsFilled() Then
            MsgBox("Please fill in all the customer details.")
            Exit Sub
        End If

        Try
            Dim age As Integer = AgeCalculator(DTP_DOBPtn)
            If con.State = ConnectionState.Open Then
                con.Close()
            End If

            con.Open()
            Dim query As String = "UPDATE CustomerData SET Name = @Name, DOB = @DOB, Age = @Age, Gender = @Gender, BloodGroup = @BloodGroup, PhoneNo = @PhoneNo, Address = @Address WHERE PatientID = @PatientID  "

            Using cmd As New SqlCommand(query, con)
                ' Set parameter values...'
                cmd.Parameters.AddWithValue("@Name", If(Not String.IsNullOrEmpty(Txt_PtnName.Text), Txt_PtnName.Text, DBNull.Value))
                cmd.Parameters.AddWithValue("@DOB", DTP_DOBPtn.Value.Date)
                cmd.Parameters.AddWithValue("@Age", age)
                cmd.Parameters.AddWithValue("@Gender", If(Txt_PtnGender.SelectedIndex <> -1, Txt_PtnGender.SelectedItem.ToString(), DBNull.Value))
                cmd.Parameters.AddWithValue("@BloodGroup", If(Txt_PtnBloodGrp.SelectedIndex <> -1, Txt_PtnBloodGrp.SelectedItem.ToString(), DBNull.Value))
                cmd.Parameters.AddWithValue("@PhoneNo", If(Not String.IsNullOrEmpty(Txt_PtnPhnNo.Text), Txt_PtnPhnNo.Text, DBNull.Value))
                cmd.Parameters.AddWithValue("@Address", If(Not String.IsNullOrEmpty(Txt_PtnAddress.Text), Txt_PtnAddress.Text, DBNull.Value))
                cmd.Parameters.AddWithValue("@PatientID", Txt_Ptnid.Text) ' Assuming customerID is the ID of the customer to be updated

                ' Execute the query...'
                cmd.ExecuteNonQuery()

                ' Display success message...'
                MsgBox("Patient: " & Txt_PtnName.Text & " Data has been updated successfully ")

                ' Clear controls...
                ClearAllPTNControls()
            End Using
        Catch ex As Exception
            ' Display error message...
            MsgBox("Error: " & ex.Message)
        Finally
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
        End Try

        Populatedvg(con, "CustomerData", DataGridView1)
    End Sub

    Private Sub ClearAllPTNControls()
        ' Clear all input controls
        Dim controlsToClear() As Control = {Txt_PtnName, Txt_PtnPhnNo, Txt_PtnAddress, Txt_Ptnid}
        For Each control As Control In controlsToClear
            control.Text = String.Empty
        Next
        Txt_PtnGender.SelectedIndex = -1
        Txt_PtnBloodGrp.SelectedIndex = -1
        DTP_DOBPtn.Value = Date.Today
    End Sub



    Private Sub Guna2GradientButton2_Click(sender As Object, e As EventArgs) Handles Guna2GradientButton2.Click
        If Not IsAllPTNFieldsFilled() Then
            MsgBox("Please fill in all the customer details.")
            Exit Sub
        End If
        If CHECKeMAIL(con, Txt_PtnPhnNo.Text, "CustomerData", "PhoneNo") = True Then
            MsgBox("Staff With This Phone number Already Exist..")

            Exit Sub
        End If

        Try
            Dim age As Integer = AgeCalculator(DTP_DOBPtn)
            If con.State = ConnectionState.Open Then
                con.Close()
            End If

            con.Open()
            Dim query As String = "INSERT INTO CustomerData (Name,  DOB, Age,Gender,BloodGroup, PhoneNo,Address) " &
                        "VALUES (@Name,  @DOB, @Age,@Gender,@BloodGroup, @PhoneNo,@Address)"
            Using cmd As New SqlCommand(query, con)
                ' Set parameter values...'
                cmd.Parameters.AddWithValue("@Name", If(Not String.IsNullOrEmpty(Txt_PtnName.Text), Txt_PtnName.Text, DBNull.Value))
                cmd.Parameters.AddWithValue("@DOB", DTP_DOBPtn.Value.Date)
                cmd.Parameters.AddWithValue("@Age", age)
                cmd.Parameters.AddWithValue("@Gender", If(Txt_PtnGender.SelectedIndex <> -1, Txt_PtnGender.SelectedItem.ToString(), DBNull.Value))
                cmd.Parameters.AddWithValue("@BloodGroup", If(Txt_PtnBloodGrp.SelectedIndex <> -1, Txt_PtnBloodGrp.SelectedItem.ToString(), DBNull.Value))
                cmd.Parameters.AddWithValue("@PhoneNo", If(Not String.IsNullOrEmpty(Txt_PtnPhnNo.Text), Txt_PtnPhnNo.Text, DBNull.Value))
                cmd.Parameters.AddWithValue("@Address", If(Not String.IsNullOrEmpty(Txt_PtnAddress.Text), Txt_PtnAddress.Text, DBNull.Value))
                ' Execute the query...'
                cmd.ExecuteNonQuery()

                ' Display success message...'
                MsgBox("New Patients : " & Txt_PtnName.Text & " Data has been added Successfully ")

                ' Clear controls...
                ClearAllPTNControls()
            End Using
        Catch ex As Exception
            ' Display error message...
            MsgBox("Error: " & ex.Message)
        Finally
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
        End Try
        Populatedvg(con, "CustomerData", DataGridView1)

    End Sub

    Private Sub Guna2GradientButton4_Click(sender As Object, e As EventArgs) Handles Guna2GradientButton4.Click
        ClearAllPTNControls()
    End Sub

    Private Sub Guna2GradientButton20_Click(sender As Object, e As EventArgs) Handles Guna2GradientButton20.Click
        If Not String.IsNullOrEmpty(Txt_Ptnid.Text) Then
            con.Open()
            Try
                ' Check if the patient ID exists in the database
                Dim query = "SELECT * FROM CustomerData WHERE PatientID = @PatientID"
                Using cmd As New SqlCommand(query, con)
                    cmd.Parameters.AddWithValue("@PatientID", Txt_Ptnid.Text)

                    ' Execute the query and check if any rows were returned
                    Dim reader As SqlDataReader = cmd.ExecuteReader()
                    If reader.HasRows Then
                        reader.Read()
                        Txt_PtnName.Text = reader(1) ' Name
                        DTP_DOBPtn.Value = reader(2) ' Date of Birth
                        Txt_PtnGender.Text = reader(4) ' gender
                        Txt_PtnBloodGrp.Text = reader(5) ' PhnNo
                        Txt_PtnPhnNo.Text = reader(6) ' PhnNo
                        Txt_PtnAddress.Text = reader(7) ' Address

                    Else
                        MessageBox.Show("Patient ID number not found.")
                    End If
                End Using
            Catch ex As Exception
                ' Handle the exception (e.g., display an error message)
                MessageBox.Show("An error occurred: " & ex.Message)
            Finally
                con.Close()
            End Try
        Else
            MessageBox.Show("Please enter a Patient ID number.")
        End If
    End Sub

    Private Sub Guna2GradientButton1_Click(sender As Object, e As EventArgs) Handles Guna2GradientButton1.Click
        Try
            If con.State = ConnectionState.Open Then
                con.Close()
            End If

            If Not String.IsNullOrEmpty(TextBox5.Text) Then
                DataGridView1.Columns.Clear()
                con.Open()

                Dim sql As String = ""
                Dim Colmntype As String = "PatientID"


                Dim tablename As String = "CustomerData"

                sql = "SELECT * FROM " & tablename & " WHERE " & Colmntype & " = @Condition"

                Dim adapter As SqlDataAdapter
                adapter = New SqlDataAdapter(sql, con)

                adapter.SelectCommand.Parameters.AddWithValue("@Condition", TextBox5.Text)
                Dim builder As SqlCommandBuilder
                builder = New SqlCommandBuilder(adapter)

                Dim ds As DataSet
                ds = New DataSet
                adapter.Fill(ds)

                DataGridView1.DataSource = ds.Tables(0)
            Else
                MsgBox("Please Enter the Patient ID To Search")
            End If
        Catch ex As Exception
            MessageBox.Show("An error occurred: " & ex.Message)
        Finally
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
        End Try
    End Sub
    Private Function IsAllStaffFieldsFilled() As Boolean
        ' Check if all required fields are filled
        Return Not (
            String.IsNullOrWhiteSpace(Txt_Staffaddress.Text) OrElse
            String.IsNullOrWhiteSpace(Txt_StaffEmail.Text) OrElse
            String.IsNullOrWhiteSpace(Txt_StaffNAme.Text) OrElse
            String.IsNullOrWhiteSpace(Txt_StaffPhnNo.Text) OrElse
            Txt_StaffGender.SelectedIndex = -1 OrElse
            Txt_Staffdesgnation.SelectedIndex = -1)
        ' Optionally, you can remove additional conditions (CheckBoxInternetBanking, CheckBoxMobilrBanking, CheckBoxChequebook, CheckBoxemailAlerts, CheckBoxestatement)
    End Function

    Private Sub ClearAllStaffControls()
        ' Clear all input controls
        Dim controlsToClear() As Control = {Txt_Staffaddress, Txt_StaffEmail, Txt_StaffId, Txt_StaffPhnNo, Txt_StaffNAme, Txt_StaffRoomNo}
        For Each control As Control In controlsToClear
            control.Text = String.Empty
        Next
        Txt_StaffGender.SelectedIndex = -1
        Txt_StaffsplswdIn.SelectedIndex = -1
        Txt_Staffdesgnation.SelectedIndex = -1
        DTP_StffDob.Value = Date.Today
        DTP_StffJoiningDate.Value = Date.Today
    End Sub
    Private Sub Guna2GradientButton8_Click(sender As Object, e As EventArgs) Handles Guna2GradientButton8.Click
        If Not IsAllStaffFieldsFilled() Then
            MsgBox("Please fill in all the Staff details.")
            Exit Sub
        End If
        If CHECKeMAIL(con, Txt_StaffPhnNo.Text, "StaffData", "PhoneNo") = True Then
            MsgBox("Staff With This Phone number Already Exist..")
            Exit Sub
        End If
        Try
            Dim age As Integer = AgeCalculator(DTP_StffDob)
            Dim CusPassWord As String = GeneratePassword(Txt_StaffNAme)

            If con.State = ConnectionState.Open Then
                con.Close()
            End If

            con.Open()
            Dim query As String = "INSERT INTO StaffData (Name,  DOB, Age,Gender,PhoneNo,Email,Password,JoiningDate,Desgnation,Specialites,Address,RoomNo) " &
                        "VALUES (@Name,  @DOB, @Age,@Gender,@PhoneNo,@Email,@Password,@JoiningDate,@Desgnation,@Specialites,@Address,@RoomNo)"
            Using cmd As New SqlCommand(query, con)
                ' Set parameter values...'
                cmd.Parameters.AddWithValue("@Name", If(Not String.IsNullOrEmpty(Txt_StaffNAme.Text), Txt_StaffNAme.Text, DBNull.Value))
                cmd.Parameters.AddWithValue("@DOB", DTP_StffDob.Value.Date)
                cmd.Parameters.AddWithValue("@Age", age)
                cmd.Parameters.AddWithValue("@Gender", If(Txt_StaffGender.SelectedIndex <> -1, Txt_StaffGender.SelectedItem.ToString(), DBNull.Value))
                cmd.Parameters.AddWithValue("@PhoneNo", If(Not String.IsNullOrEmpty(Txt_StaffPhnNo.Text), Txt_StaffPhnNo.Text, DBNull.Value))
                cmd.Parameters.AddWithValue("@Email", If(Not String.IsNullOrEmpty(Txt_StaffEmail.Text), Txt_StaffEmail.Text, DBNull.Value))
                cmd.Parameters.AddWithValue("@Password", If(Not String.IsNullOrEmpty(CusPassWord), CusPassWord, DBNull.Value))
                cmd.Parameters.AddWithValue("@JoiningDate", DTP_StffJoiningDate.Value.Date)
                cmd.Parameters.AddWithValue("@Desgnation", If(Txt_Staffdesgnation.SelectedIndex <> -1, Txt_Staffdesgnation.SelectedItem.ToString(), DBNull.Value))
                cmd.Parameters.AddWithValue("@Specialites", If(Txt_StaffsplswdIn.SelectedIndex <> -1, Txt_StaffsplswdIn.SelectedItem.ToString(), DBNull.Value))
                cmd.Parameters.AddWithValue("@Address", If(Not String.IsNullOrEmpty(Txt_Staffaddress.Text), Txt_Staffaddress.Text, DBNull.Value))
                cmd.Parameters.AddWithValue("@RoomNo", If(Not String.IsNullOrEmpty(Txt_StaffRoomNo.Text), Txt_StaffRoomNo.Text, DBNull.Value))
                ' Execute the query...'
                cmd.ExecuteNonQuery()

                ' Display success message...'
                MsgBox("New Staff : " & Txt_StaffNAme.Text & " Data has been added Successfully ")

                ' Clear controls...
                ClearAllStaffControls()
            End Using
        Catch ex As Exception
            ' Display error message...
            MsgBox("Error: " & ex.Message)
        Finally
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
        End Try
        FillSelectedColumnIncomboBox(con, Txt_AppntDoctorName, "StaffData", "Name", "Desgnation", "Doctor")
        Populatedvg(con, "StaffData", DataGridView2)
    End Sub

    Private Sub Guna2GradientButton7_Click(sender As Object, e As EventArgs) Handles Guna2GradientButton7.Click
        If Not IsAllStaffFieldsFilled() Then
            MsgBox("Please fill in all the Staff details.")
            Exit Sub
        End If

        Try
            Dim age As Integer = AgeCalculator(DTP_StffDob)
            Dim CusPassWord As String = GeneratePassword(Txt_StaffNAme)

            If con.State = ConnectionState.Open Then
                con.Close()
            End If

            con.Open()
            Dim query As String = "UPDATE StaffData SET Name = @Name, DOB = @DOB, Age = @Age, Gender = @Gender, PhoneNo = @PhoneNo, Email = @Email ," &
                          "Password=@Password,JoiningDate = @JoiningDate, Desgnation = @Desgnation, Specialites = @Specialites, Address = @Address, RoomNo = @RoomNo " &
                          "WHERE StaffID = @StaffID" ' Assuming StaffID is the primary key to identify staff members

            Using cmd As New SqlCommand(query, con)
                ' Set parameter values...'
                cmd.Parameters.AddWithValue("@Name", If(Not String.IsNullOrEmpty(Txt_StaffNAme.Text), Txt_StaffNAme.Text, DBNull.Value))
                cmd.Parameters.AddWithValue("@DOB", DTP_StffDob.Value.Date)
                cmd.Parameters.AddWithValue("@Age", age)
                cmd.Parameters.AddWithValue("@Gender", If(Txt_StaffGender.SelectedIndex <> -1, Txt_StaffGender.SelectedItem.ToString(), DBNull.Value))
                cmd.Parameters.AddWithValue("@PhoneNo", If(Not String.IsNullOrEmpty(Txt_StaffPhnNo.Text), Txt_StaffPhnNo.Text, DBNull.Value))
                cmd.Parameters.AddWithValue("@Email", If(Not String.IsNullOrEmpty(Txt_StaffEmail.Text), Txt_StaffEmail.Text, DBNull.Value))
                cmd.Parameters.AddWithValue("@Password", CusPassWord)
                cmd.Parameters.AddWithValue("@JoiningDate", DTP_StffJoiningDate.Value.Date)
                cmd.Parameters.AddWithValue("@Desgnation", If(Txt_Staffdesgnation.SelectedIndex <> -1, Txt_Staffdesgnation.SelectedItem.ToString(), DBNull.Value))
                cmd.Parameters.AddWithValue("@Specialites", If(Txt_StaffsplswdIn.SelectedIndex <> -1, Txt_StaffsplswdIn.SelectedItem.ToString(), DBNull.Value))
                cmd.Parameters.AddWithValue("@Address", If(Not String.IsNullOrEmpty(Txt_Staffaddress.Text), Txt_Staffaddress.Text, DBNull.Value))
                cmd.Parameters.AddWithValue("@StaffID", Txt_StaffId.Text) ' Replace staffID with the actual ID of the staff member you want to update
                cmd.Parameters.AddWithValue("@RoomNo", If(Not String.IsNullOrEmpty(Txt_StaffRoomNo.Text), Txt_StaffRoomNo.Text, DBNull.Value))

                ' Execute the query...'
                cmd.ExecuteNonQuery()

                ' Display success message...'
                MsgBox("Staff data for " & Txt_StaffNAme.Text & " has been updated successfully")

                ' Clear controls...
                ClearAllStaffControls()
            End Using
        Catch ex As Exception
            ' Display error message...
            MsgBox("Error: " & ex.Message)
        Finally
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
        End Try
        FillSelectedColumnIncomboBox(con, Txt_AppntDoctorName, "StaffData", "Name", "Desgnation", "Doctor")
        Populatedvg(con, "StaffData", DataGridView2)

    End Sub

    Private Sub Guna2GradientButton6_Click(sender As Object, e As EventArgs) Handles Guna2GradientButton6.Click
        ClearAllStaffControls()
    End Sub

    Private Sub Guna2GradientButton24_Click(sender As Object, e As EventArgs) Handles Guna2GradientButton24.Click
        If Not String.IsNullOrEmpty(Txt_StaffId.Text) Then

            con.Open()
            Try
                ' Check if the staff ID exists in the database
                Dim query As String = "SELECT * FROM StaffData WHERE StaffId = @StaffId"
                Using cmd As New SqlCommand(query, con)
                    cmd.Parameters.AddWithValue("@StaffId", Txt_StaffId.Text)

                    ' Execute the query and check if any rows were returned
                    Dim reader As SqlDataReader = cmd.ExecuteReader()
                    If reader.HasRows Then
                        reader.Read()
                        ' Populate the fields with retrieved data
                        Txt_StaffNAme.Text = reader(1) ' Name
                        DTP_StffDob.Value = reader(2) ' Date of Birth
                        Txt_StaffGender.Text = reader(4) ' Gender
                        Txt_StaffPhnNo.Text = reader(5) ' Phone Number
                        Txt_StaffEmail.Text = reader(6) ' Email
                        DTP_StffJoiningDate.Value = reader(8) ' Date of Joining
                        Txt_Staffdesgnation.Text = reader(9) ' Designation
                        ' Check for DBNull in Specialised field (index 10)
                        If Not reader.IsDBNull(10) Then
                            If Not String.IsNullOrEmpty(reader(10)) Then
                                Txt_StaffsplswdIn.Text = reader(10).ToString() ' Set Txt_StaffSpecialised text to reader(10) value
                            Else
                                Txt_StaffsplswdIn.Text = "" ' Set Txt_StaffSpecialised text to empty if reader(10) is empty string
                            End If
                        Else
                            Txt_StaffsplswdIn.Text = "" ' Set Txt_StaffSpecialised text to empty if reader(10) is DBNull
                        End If


                        Txt_Staffaddress.Text = reader(11) ' Address

                        ' Check for DBNull in Room Number field (index 12)
                        If Not reader.IsDBNull(12) AndAlso Not String.IsNullOrEmpty(reader(12).ToString()) Then
                            Txt_StaffRoomNo.Text = reader(12) ' Set Txt_StaffRoomNo text to reader(12) value
                        Else
                            Txt_StaffRoomNo.Text = String.Empty ' Set Txt_StaffRoomNo text to empty if reader(12) is DBNull or empty
                        End If
                    Else
                        MessageBox.Show("Staff ID number not found.")
                    End If
                End Using
            Catch ex As Exception
                ' Handle the exception (e.g., display an error message)
                MessageBox.Show("An error occurred: " & ex.Message)
            Finally
                con.Close()
            End Try
        Else
            MessageBox.Show("Please enter a Staff ID number.")
        End If

        Populatedvg(con, "StaffData", DataGridView2)
    End Sub

    Private Sub Guna2GradientButton5_Click(sender As Object, e As EventArgs) Handles Guna2GradientButton5.Click
        Try
            If con.State = ConnectionState.Open Then
                con.Close()
            End If

            If Not String.IsNullOrEmpty(TextBox3.Text) Then
                DataGridView2.Columns.Clear()
                con.Open()

                Dim sql As String = ""
                Dim Colmntype As String = "StaffId"


                Dim tablename As String = "StaffData"

                sql = "SELECT * FROM " & tablename & " WHERE " & Colmntype & " = @Condition"

                Dim adapter As SqlDataAdapter
                adapter = New SqlDataAdapter(sql, con)

                adapter.SelectCommand.Parameters.AddWithValue("@Condition", TextBox3.Text)
                Dim builder As SqlCommandBuilder
                builder = New SqlCommandBuilder(adapter)

                Dim ds As DataSet
                ds = New DataSet
                adapter.Fill(ds)

                DataGridView2.DataSource = ds.Tables(0)
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

    Private Sub Txt_Staffdesgnation_SelectedIndexChanged(sender As Object, e As EventArgs) Handles Txt_Staffdesgnation.SelectedIndexChanged
        If Txt_Staffdesgnation.SelectedIndex = 0 Then
            Txt_StaffsplswdIn.Enabled = True
        ElseIf Txt_Staffdesgnation.SelectedIndex = 1 Then
            Txt_StaffsplswdIn.Enabled = False
            Txt_StaffsplswdIn.SelectedIndex = -1
        End If
    End Sub

    Private Sub CheckBox1_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox1.CheckedChanged
        If CheckBox1.Checked Then
            Txt_TestID.Enabled = True
        Else
            Txt_TestID.Enabled = False
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
    Private Sub Guna2GradientButton12_Click(sender As Object, e As EventArgs) Handles Guna2GradientButton12.Click
        If Not IsAllTestFieldsFilled() Then
            MsgBox("Please fill in all the Test details.")
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
        FillcomboBox(con, Txt_AppntTestName, "TestData", "TestName")

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

        FillcomboBox(con, Txt_AppntTestName, "TestData", "TestName")
        Populatedvg(con, "TestData", DataGridView3)
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
            Txt_AppntDoctorName.SelectedIndex = -1 OrElse
            Txt_AppntTestName.SelectedIndex = -1
           )
        ' Optionally, you can remove additional conditions (CheckBoxInternetBanking, CheckBoxMobilrBanking, CheckBoxChequebook, CheckBoxemailAlerts, CheckBoxestatement)
    End Function

    Private Sub ClearAllAppntControls()
        ' Clear all input controls
        Dim controlsToClear() As Control = {Txt_AppntDoctorID, Txt_AppntDoctorRoomNo, Txt_AppntPtnID, Txt_AppntPtnName, Txt_AppntPtnPhnNumber, Txt_AppntTestCost, Txt_AppntTestID}
        For Each control As Control In controlsToClear
            control.Text = String.Empty
        Next
        Txt_AppntDoctorName.SelectedIndex = -1
        Txt_AppntTestName.SelectedIndex = -1
    End Sub

    Private Shared current_date As Date = Date.Today
    Private Shared token As Integer = 1

    Private Function GetLastTokenFromDatabase() As Integer
        Dim lastToken As Integer = 0

        Try
            con.Open()

            Dim query As String = "SELECT MAX(TockenNo) AS LastToken FROM AppointMentData"
            Dim cmd As New SqlCommand(query, con)

            Dim result As Object = cmd.ExecuteScalar()
            If result IsNot Nothing AndAlso Not IsDBNull(result) Then
                Integer.TryParse(result.ToString(), lastToken)
            End If
        Catch ex As Exception
            MessageBox.Show("Error: " & ex.Message)
        Finally
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
        End Try

        Return lastToken
    End Function

    Private Function GenerateTokenNumber() As Integer
        Dim newDate As Date = Date.Today

        ' Retrieve the last token number from the database
        Dim lastDBToken As Integer = GetLastTokenFromDatabase()

        ' If a valid token is retrieved from the database, update the current token
        If lastDBToken > 0 Then
            token = lastDBToken + 1
        Else
            token = 1
        End If

        ' Check if the date has changed
        If newDate <> current_date Then
            ' If the date has changed, reset token to 1 and update current_date
            token = 1
            current_date = newDate

            ' Clear all data from the AppointMentData table
            Try
                con.Open()

                Dim query As String = "TRUNCATE TABLE AppointMentData"
                Dim cmd As New SqlCommand(query, con)
                cmd.ExecuteNonQuery()

                MessageBox.Show("All data cleared from table: AppointMentData")
            Catch ex As Exception
                MessageBox.Show("Error: " & ex.Message)
            Finally
                If con.State = ConnectionState.Open Then
                    con.Close()
                End If
            End Try
        End If

        Return token
    End Function

    ' lets declare some global variable for print purpose
    Dim WithEvents PD As New PrintDocument
    Dim PPD As New PrintPreviewDialog
    Dim LongPaper As Integer

    Private Sub PD_BeginPrint(sender As Object, e As Printing.PrintEventArgs) Handles PD.BeginPrint
        Dim PageSetUp As New PageSettings
        Dim CustomPaperSize As New PaperSize("Custom", 400, 400)
        PageSetUp.PaperSize = CustomPaperSize ' Assign the custom PaperSize object to PageSetUp.PaperSize
        PD.DefaultPageSettings = PageSetUp ' Assuming PD is your PrintDocument object
    End Sub

    Private Sub PD_PrintPage(sender As Object, e As Printing.PrintPageEventArgs) Handles PD.PrintPage
        Dim f7 As New Font("Rockwell", 7, FontStyle.Regular)
        Dim f8 As New Font("Rockwell", 8, FontStyle.Regular)
        Dim f10 As New Font("Rockwell", 10, FontStyle.Regular)
        Dim f10b As New Font("Rockwell", 10, FontStyle.Bold)
        Dim f14 As New Font("Rockwell", 14, FontStyle.Regular)
        Dim f16 As New Font("Rockwell", 16, FontStyle.Bold)

        Dim leftMargin As Integer = e.MarginBounds.Left
        Dim CenterMargin As Integer = 200
        Dim RightMargin As Integer = e.MarginBounds.Right

        ' Font alignment
        Dim right As New StringFormat
        Dim Center As New StringFormat
        Dim left As New StringFormat
        right.Alignment = StringAlignment.Far
        Center.Alignment = StringAlignment.Center
        left.Alignment = StringAlignment.Near

        Dim line As String = "----------------------------------------------------------------------------------------------------"
        Dim starline As String = "************************************************************************************************"

        Dim content As String = " "

        'e.Graphics.DrawString("Right-aligned text", Font, Brush, x, y, right)
        ' e.Graphics.DrawString("Centered text", Font, Brush, x, y, Center)
        ' e.Graphics.DrawString("Left-aligned text", Font, Brush, x, y, left)



        ' Example content to print
        Dim textToPrint As String = "Where Precision Meets Compassion: PrimeCare, Your Health's Best Companion!"

        ' Drawing text on the print document
        e.Graphics.DrawString("PrimeCare Diagnostic Center", f14, Brushes.Black, CenterMargin, 5, Center)
        e.Graphics.DrawString("Near Sector 9 Bhilai", f8, Brushes.Black, CenterMargin, 25, Center)
        e.Graphics.DrawString("Phone No : +919586847588", f8, Brushes.Black, leftMargin, 45, Center)
        e.Graphics.DrawString(textToPrint, f7, Brushes.Black, CenterMargin, 60, Center)
        e.Graphics.DrawString(line, f10, Brushes.Black, CenterMargin, 70, Center)
        e.Graphics.DrawString("We Always try to Serve Our Patients Best", f10, Brushes.Black, CenterMargin, 85, Center)
        e.Graphics.DrawString(starline, f14, Brushes.Black, CenterMargin, 100, Center)
        e.Graphics.DrawString("Tocken Number :             " & globaltockenNo, f10b, Brushes.Black, 200, 115, Center)
        e.Graphics.DrawString("Patient ID :                       " & Txt_AppntPtnID.Text, f8, Brushes.Black, leftMargin, 135, left)
        e.Graphics.DrawString("Patient Name :                 " & Txt_AppntPtnName.Text, f8, Brushes.Black, leftMargin, 150, left)
        e.Graphics.DrawString("Patient Phone number :   " & Txt_AppntPtnPhnNumber.Text, f8, Brushes.Black, leftMargin, 165, left)
        e.Graphics.DrawString("Docter Name :                  " & Txt_AppntDoctorName.Text, f8, Brushes.Black, leftMargin, 180, left)
        e.Graphics.DrawString("Room No :                         " & Txt_AppntDoctorRoomNo.Text, f8, Brushes.Black, leftMargin, 195, left)
        e.Graphics.DrawString("Test Name :                      " & Txt_AppntTestName.Text, f8, Brushes.Black, leftMargin, 210, left)
        e.Graphics.DrawString("Test Cost :                        " & Txt_AppntTestCost.Text, f8, Brushes.Black, leftMargin, 225, left)
        e.Graphics.DrawString(starline, f14, Brushes.Black, CenterMargin, 250, Center)
        e.Graphics.DrawString("Thanks for Choosing Us", f10b, Brushes.Black, CenterMargin, 265, Center)
        e.Graphics.DrawString(line, f10, Brushes.Black, CenterMargin, 280, Center)
        e.Graphics.DrawString("PrimeCare Diagnostic Center: Your destination for comprehensive scans and tests.", f7, Brushes.Black, CenterMargin, 295, Center) ' Adjust for spacing
        e.Graphics.DrawString("With advanced technology and a dedicated team, we offer MRI, CT scans, ", f7, Brushes.Black, CenterMargin, 305, Center) ' Adjust for spacing
        e.Graphics.DrawString("blood tests and more. Experience precise diagnostics and compassionate", f7, Brushes.Black, CenterMargin, 315, Center) ' Adjust for spacing
        e.Graphics.DrawString(" care tailored to your needs.", f7, Brushes.Black, CenterMargin, 325, Center) ' Adjust for spacing
        ' Indicate that there is no more content to print
        e.HasMorePages = False
    End Sub



    Private Sub Guna2GradientButton22_Click(sender As Object, e As EventArgs) Handles Guna2GradientButton22.Click
        If Not IsAllAttntFieldsFilled() Then
            MsgBox("Please fill in all the Appointment details.")
            Exit Sub
        End If

        Try
            Dim tockenNo As Integer = GenerateTokenNumber()
            globaltockenNo = tockenNo
            If con.State = ConnectionState.Open Then
                con.Close()
            End If

            con.Open()
            Dim query As String = "INSERT INTO AppointMentData (PatientName, PatientID, PatientPhoneNumber,TestName,TestID,TestCost,DoctorName,DoctorID,DoctotRoomNo,TockenNo) " &
                        "VALUES (@PatientName, @PatientID, @PatientPhoneNumber,@TestName,@TestID,@TestCost,@DoctorName,@DoctorID,@DoctotRoomNo,@TockenNo)"
            Using cmd As New SqlCommand(query, con)
                ' Set parameter values...'
                cmd.Parameters.AddWithValue("@PatientName", If(Not String.IsNullOrEmpty(Txt_AppntPtnName.Text), Txt_AppntPtnName.Text, DBNull.Value))
                cmd.Parameters.AddWithValue("@PatientID", If(Not String.IsNullOrEmpty(Txt_AppntPtnID.Text), Txt_AppntPtnID.Text, DBNull.Value))
                cmd.Parameters.AddWithValue("@PatientPhoneNumber", If(Not String.IsNullOrEmpty(Txt_AppntPtnPhnNumber.Text), Txt_AppntPtnPhnNumber.Text, DBNull.Value))
                cmd.Parameters.AddWithValue("@TestName", If(Txt_AppntTestName.SelectedIndex <> -1, Txt_AppntTestName.SelectedValue.ToString(), DBNull.Value))
                cmd.Parameters.AddWithValue("@TestID", If(Not String.IsNullOrEmpty(Txt_AppntTestID.Text), Txt_AppntTestID.Text, DBNull.Value))
                cmd.Parameters.AddWithValue("@TestCost", If(Not String.IsNullOrEmpty(Txt_AppntTestCost.Text), Txt_AppntTestCost.Text, DBNull.Value))
                cmd.Parameters.AddWithValue("@DoctorName", If(Txt_AppntDoctorName.SelectedIndex <> -1, Txt_AppntDoctorName.SelectedValue.ToString(), DBNull.Value))
                cmd.Parameters.AddWithValue("@DoctorID", If(Not String.IsNullOrEmpty(Txt_AppntDoctorID.Text), Txt_AppntDoctorID.Text, DBNull.Value))
                cmd.Parameters.AddWithValue("@DoctotRoomNo", If(Not String.IsNullOrEmpty(Txt_AppntDoctorRoomNo.Text), Txt_AppntDoctorRoomNo.Text, DBNull.Value))
                cmd.Parameters.AddWithValue("@TockenNo", tockenNo)
                ' Execute the query...'
                cmd.ExecuteNonQuery()

                ' Display success message...'
                MsgBox("Patient : " & Txt_AppntPtnName.Text & " Appointment Data has been added Successfully ")
                PPD.Document = PD
                PPD.ShowDialog()
                PD.Print()
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

    Private Sub Txt_AppntPtnID_TextChanged(sender As Object, e As EventArgs) Handles Txt_AppntPtnID.TextChanged
        If Not String.IsNullOrEmpty(Txt_AppntPtnID.Text) Then
            con.Open()
            Try
                ' Check if the staff ID exists in the database
                Dim query As String = "SELECT * FROM CustomerData WHERE PatientID = @PatientID"
                Using cmd As New SqlCommand(query, con)
                    cmd.Parameters.AddWithValue("@PatientID", Txt_AppntPtnID.Text)

                    ' Execute the query and check if any rows were returned
                    Dim reader As SqlDataReader = cmd.ExecuteReader()
                    If reader.HasRows Then
                        reader.Read()
                        ' Populate the fields with retrieved data
                        Txt_AppntPtnName.Text = reader(1) ' Name
                        Txt_AppntPtnPhnNumber.Text = reader(6) ' Email
                    End If
                End Using
            Catch ex As Exception
                ' Handle the exception (e.g., display an error message)
                MessageBox.Show("An error occurred: " & ex.Message)
            Finally
                con.Close()
            End Try
        ElseIf Txt_AppntPtnID.Text = "" Then
            Txt_AppntPtnName.Clear()
            Txt_AppntPtnPhnNumber.Clear()
        End If


    End Sub

    Private Sub Txt_AppntTestName_SelectionChangeCommitted(sender As Object, e As EventArgs) Handles Txt_AppntTestName.SelectionChangeCommitted
        Try
            con.Open()

            ' Fill the ComboBox with data

            ' Check if a value is selected in the ComboBox
            If Txt_AppntTestName.SelectedIndex <> -1 Then
                Dim Query As String = "SELECT * FROM TestData WHERE TestName = @TestName"
                Dim cmd As New SqlCommand(Query, con)
                cmd.Parameters.AddWithValue("@TestName", Txt_AppntTestName.SelectedValue.ToString())

                Dim reader As SqlDataReader = cmd.ExecuteReader()

                If reader.Read() Then
                    Txt_AppntTestID.Text = reader(0).ToString()
                    Txt_AppntTestCost.Text = reader(3).ToString()
                End If

                reader.Close()
            End If
        Catch ex As Exception
            ' Handle exceptions (display or log error message)
            MessageBox.Show("Error: " & ex.Message)
        Finally
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
        End Try
    End Sub

    Private Sub Txt_AppntDoctorName_SelectionChangeCommitted(sender As Object, e As EventArgs) Handles Txt_AppntDoctorName.SelectionChangeCommitted
        Try
            con.Open()

            ' Fill the ComboBox with data

            ' Check if a value is selected in the ComboBox
            If Txt_AppntDoctorName.SelectedIndex <> -1 Then
                Dim Query As String = "SELECT * FROM StaffData WHERE Name = @Name"
                Dim cmd As New SqlCommand(Query, con)
                cmd.Parameters.AddWithValue("@Name", Txt_AppntDoctorName.SelectedValue.ToString())

                Dim reader As SqlDataReader = cmd.ExecuteReader()

                If reader.Read() Then
                    Txt_AppntDoctorID.Text = reader(0).ToString()
                    Txt_AppntDoctorRoomNo.Text = reader(12).ToString()
                End If

                reader.Close()
            End If
        Catch ex As Exception
            ' Handle exceptions (display or log error message)
            MessageBox.Show("Error: " & ex.Message)
        Finally
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
        End Try
    End Sub

    Private Sub Guna2GradientButton21_Click(sender As Object, e As EventArgs) Handles Guna2GradientButton21.Click
        ClearAllAppntControls()

    End Sub

    Private Sub Guna2GradientButton23_Click(sender As Object, e As EventArgs) Handles Guna2GradientButton23.Click
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
                MsgBox("Please Enter the Reference ID To Search")
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

    Private Sub Guna2GradientButton14_Click(sender As Object, e As EventArgs) Handles Guna2GradientButton14.Click
        Try
            ' Close the connection if it's already open
            If con.State = ConnectionState.Open Then
                con.Close()
            End If

            ' Clear previous data in DataGridView3
            DataGridView5.DataSource = Nothing
            DataGridView5.Columns.Clear()

            ' Check if TextBox38 is not empty
            If Not String.IsNullOrEmpty(TextBox17.Text) Then
                ' Open the connection
                con.Open()

                Dim sql As String = ""
                Dim Colmntype As String = ""
                Dim tablename As String = "ReportData"


                ' Determine the column based on ComboBox selection
                If ComboBox7.SelectedIndex = 0 Then
                    Colmntype = "PatientName"
                ElseIf ComboBox7.SelectedIndex = 1 Then
                    Colmntype = "DoctorName"
                ElseIf ComboBox7.SelectedIndex = 2 Then
                    Colmntype = "TestName"
                End If

                ' Create the SQL query with a parameterized query
                sql = "SELECT * FROM " & tablename & " WHERE " & Colmntype & " = @Condition"

                ' Create SQL data adapter
                Dim adapter As SqlDataAdapter = New SqlDataAdapter(sql, con)
                adapter.SelectCommand.Parameters.AddWithValue("@Condition", TextBox17.Text)

                ' Create DataSet to hold the fetched data
                Dim ds As DataSet = New DataSet()
                adapter.Fill(ds)

                ' Bind the data to DataGridView3
                DataGridView5.DataSource = ds.Tables(0)
            Else
                ' Show a message if TextBox38 is empty
                MsgBox("Please Enter the Reference ID To Search")
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

    Private Sub ComboBox7_SelectionChangeCommitted(sender As Object, e As EventArgs) Handles ComboBox7.SelectionChangeCommitted
        Try
            con.Open()

            Dim Colmntype As String = ""
            Dim tablename As String = "ReportData"

            ' Determine the column based on ComboBox selection
            If ComboBox7.SelectedIndex = 0 Then
                Colmntype = "PatientName"
            ElseIf ComboBox7.SelectedIndex = 1 Then
                Colmntype = "DoctorName"
            ElseIf ComboBox7.SelectedIndex = 2 Then
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

            TextBox17.AutoCompleteCustomSource = ElementsToSuggest
        Catch ex As Exception
            ' Handle exceptions (display or log error message)
            MessageBox.Show("Error: " & ex.Message)
        Finally
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
        End Try
    End Sub

    Private Sub TextBox17_TextChanged(sender As Object, e As EventArgs) Handles TextBox17.TextChanged
        If Not TextBox17.Text = "" Then
            Populatedvg(con, "ReportData", DataGridView5)


        End If
    End Sub

    Private Sub Guna2GradientButton18_Click(sender As Object, e As EventArgs) Handles Guna2GradientButton18.Click
        If Not String.IsNullOrEmpty(txt_BillReportNo.Text) Then

            con.Open()
            Try
                ' Check if the staff ID exists in the database
                Dim query As String = "SELECT * FROM ReportData WHERE ReportID = @ReportID"
                Using cmd As New SqlCommand(query, con)
                    cmd.Parameters.AddWithValue("@ReportID", txt_BillReportNo.Text)

                    ' Execute the query and check if any rows were returned
                    Dim reader As SqlDataReader = cmd.ExecuteReader()
                    If reader.HasRows Then
                        reader.Read()
                        ' Populate the fields with retrieved data
                        txt_BillTestName.Text = reader(4) ' Name
                        txt_BillCharges.Text = reader(6) ' TestType
                        txt_BillPatientID.Text = reader(2) ' Charges
                        PatientName.Text = reader(1) ' Patient Name
                        DTP_BillPayntrpt.Value = reader(10) ' Patient Name
                        PhoneNumberPymnt = reader(3)
                        TestIDPymnt = reader(5)
                        TestCostPymnt = reader(6)
                        DoctorNamePymnt = reader(7)
                        DoctorIDPymnt = reader(8)
                        DoctorRoomNoPymnt = reader(9)
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

    End Sub


    ' lets declare some global variable for print purpose
    Dim WithEvents PD2 As New PrintDocument
    Dim PPD2 As New PrintPreviewDialog
    Dim LongPaper2 As Integer

    Private Sub PD_BeginPrintBill(sender As Object, e As Printing.PrintEventArgs) Handles PD2.BeginPrint
        Dim PageSetUp As New PageSettings
        Dim CustomPaperSize As New PaperSize("Custom", 400, 400)
        PageSetUp.PaperSize = CustomPaperSize ' Assign the custom PaperSize object to PageSetUp.PaperSize
        PD2.DefaultPageSettings = PageSetUp ' Assuming PD is your PrintDocument object
    End Sub

    Private Sub PD_PrintPageBill(sender As Object, e As Printing.PrintPageEventArgs) Handles PD2.PrintPage
        ' Define fonts
        Dim f7 As New Font("Rockwell", 7, FontStyle.Regular)
        Dim f8 As New Font("Rockwell", 8, FontStyle.Regular)
        Dim f10 As New Font("Rockwell", 10, FontStyle.Regular)
        Dim f10b As New Font("Rockwell", 10, FontStyle.Bold)
        Dim f14 As New Font("Rockwell", 14, FontStyle.Regular)
        Dim f16 As New Font("Rockwell", 16, FontStyle.Bold)

        ' Define margins
        Dim leftMargin As Integer = e.MarginBounds.Left
        Dim CenterMargin As Integer = e.MarginBounds.Width \ 2 ' Center horizontally
        Dim RightMargin As Integer = e.MarginBounds.Right
        ' Font alignment
        Dim right As New StringFormat
        Dim Center As New StringFormat
        Dim left As New StringFormat
        right.Alignment = StringAlignment.Far
        Center.Alignment = StringAlignment.Center
        left.Alignment = StringAlignment.Near


        ' Define separator lines
        Dim line As String = "----------------------------------------------------------------------------------------------------"
        Dim starline As String = "************************************************************************************************"

        ' Example content to print
        Dim textToPrint As String = "Where Precision Meets Compassion: PrimeCare, Your Health's Best Companion!"



        'e.Graphics.DrawString("Right-aligned text", Font, Brush, x, y, right)
        ' e.Graphics.DrawString("Centered text", Font, Brush, x, y, Center)
        ' e.Graphics.DrawString("Left-aligned text", Font, Brush, x, y, left)




        ' Drawing text on the print document
        e.Graphics.DrawString("PrimeCare Diagnostic Center", f14, Brushes.Black, 200, 5, Center)
        e.Graphics.DrawString("Near Sector 9 Bhilai", f8, Brushes.Black, 200, 25, Center)
        e.Graphics.DrawString("Phone No : +919586847588", f8, Brushes.Black, leftMargin, 45, Center)
        e.Graphics.DrawString(textToPrint, f7, Brushes.Black, 200, 60, Center)
        e.Graphics.DrawString(line, f10, Brushes.Black, 200, 70, Center)
        e.Graphics.DrawString("We Always try to Serve Our Patients Best", f10, Brushes.Black, 200, 85, Center)
        e.Graphics.DrawString(starline, f14, Brushes.Black, 200, 100, Center)
        e.Graphics.DrawString("Patient ID :                       " & txt_BillPatientID.Text, f8, Brushes.Black, leftMargin, 135, left)
        e.Graphics.DrawString("Patient Name :                 " & PatientName.Text, f8, Brushes.Black, leftMargin, 150, left)
        e.Graphics.DrawString("Patient Phone number :   " & PhoneNumberPymnt, f8, Brushes.Black, leftMargin, 165, left)
        e.Graphics.DrawString("Docter Name :                  " & DoctorNamePymnt, f8, Brushes.Black, leftMargin, 180, left)
        e.Graphics.DrawString("Test Name :                      " & txt_BillTestName.Text, f8, Brushes.Black, leftMargin, 195, left)
        e.Graphics.DrawString("Test Cost :                        " & txt_BillCharges.Text, f8, Brushes.Black, leftMargin, 210, left)
        e.Graphics.DrawString("Payment Type :                        " & ComboBox6.SelectedItem.ToString(), f8, Brushes.Black, leftMargin, 225, left)
        e.Graphics.DrawString(line, f10, Brushes.Black, 200, 235, Center)
        e.Graphics.DrawString("Status :                        " & "Paid ", f8, Brushes.Black, leftMargin, 250, left)
        e.Graphics.DrawString(starline, f14, Brushes.Black, 200, 265, Center)
        e.Graphics.DrawString("Thanks for Choosing Us", f10b, Brushes.Black, 200, 280, Center)
        e.Graphics.DrawString("We always Bless For Your Health", f8, Brushes.Black, 200, 295, Center)
        e.Graphics.DrawString(line, f10, Brushes.Black, 200, 305, Center)
        e.Graphics.DrawString("PrimeCare Diagnostic Center: Your destination for comprehensive scans and tests.", f7, Brushes.Black, 200, 320, Center) ' Adjust for spacing
        e.Graphics.DrawString("With advanced technology and a dedicated team, we offer MRI, CT scans, ", f7, Brushes.Black, 200, 330, Center) ' Adjust for spacing
        e.Graphics.DrawString("blood tests and more. Experience precise diagnostics and compassionate", f7, Brushes.Black, 200, 340, Center) ' Adjust for spacing
        e.Graphics.DrawString(" care tailored to your needs.", f7, Brushes.Black, 200, 350, Center) ' Adjust for spacing
        ' Indicate that there is no more content to print
        e.HasMorePages = False
    End Sub



    Private Sub ClearAlReportControls()
        ' Clear all input controls
        Dim controlsToClear() As Control = {txt_BillCharges, txt_BillPatientID, Txt_TestID, txt_BillTestName, txt_BillReportNo, PatientName}
        For Each control As Control In controlsToClear
            control.Text = String.Empty
        Next
        ComboBox6.SelectedIndex = -1
    End Sub

    Private Sub Guna2GradientButton16_Click(sender As Object, e As EventArgs) Handles Guna2GradientButton16.Click
        If Not (String.IsNullOrEmpty(txt_BillReportNo.Text)) Then

            Try
                If con.State = ConnectionState.Open Then
                    con.Close()
                End If

                con.Open()
                Dim query As String = "INSERT INTO PaymentData (PatientName, PatientID, PatientPhoneNumber,TestName,TestID,TestCost,DoctorName,DoctorID,DoctotRoomNo,ReportDate,PaymentType,Status) " &
                        "VALUES (@PatientName, @PatientID, @PatientPhoneNumber,@TestName,@TestID,@TestCost,@DoctorName,@DoctorID,@DoctotRoomNo,@ReportDate,@PaymentType,@Status)"
                Using cmd As New SqlCommand(query, con)
                    ' Set parameter values...'
                    cmd.Parameters.AddWithValue("@PatientName", If(Not String.IsNullOrEmpty(PatientName.Text), PatientName.Text, DBNull.Value))
                    cmd.Parameters.AddWithValue("@PatientID", If(Not String.IsNullOrEmpty(txt_BillPatientID.Text), txt_BillPatientID.Text, DBNull.Value))
                    cmd.Parameters.AddWithValue("@PatientPhoneNumber", If(Not String.IsNullOrEmpty(PhoneNumberPymnt), PhoneNumberPymnt, DBNull.Value))
                    cmd.Parameters.AddWithValue("@TestName", If(Not String.IsNullOrEmpty(txt_BillTestName.Text), txt_BillTestName.Text, DBNull.Value))
                    cmd.Parameters.AddWithValue("@TestID", If(Not String.IsNullOrEmpty(TestIDPymnt), TestIDPymnt, DBNull.Value))
                    cmd.Parameters.AddWithValue("@TestCost", If(Not String.IsNullOrEmpty(txt_BillCharges.Text), txt_BillCharges.Text, DBNull.Value))
                    cmd.Parameters.AddWithValue("@DoctorName", If(Not String.IsNullOrEmpty(DoctorNamePymnt), DoctorNamePymnt, DBNull.Value))
                    cmd.Parameters.AddWithValue("@DoctorID", If(Not String.IsNullOrEmpty(DoctorIDPymnt), DoctorIDPymnt, DBNull.Value))
                    cmd.Parameters.AddWithValue("@DoctotRoomNo", If(Not String.IsNullOrEmpty(DoctorRoomNoPymnt), DoctorRoomNoPymnt, DBNull.Value))
                    cmd.Parameters.AddWithValue("@ReportDate", DTP_BillPayntrpt.Value.Date)
                    cmd.Parameters.AddWithValue("@PaymentType", ComboBox6.SelectedItem.ToString())
                    cmd.Parameters.AddWithValue("@Status", "Paid")
                    ' Execute the query...'
                    cmd.ExecuteNonQuery()

                    ' Display success message...'
                    MsgBox("Patient : " & txt_BillReportNo.Text & " ReportData Data has been added Successfully ")
                    Dim deleteQuery As String = "DELETE FROM ReportData WHERE ReportID = @ReportID"
                    Dim cmdDelete As New SqlCommand(deleteQuery, con)
                    cmdDelete.Parameters.AddWithValue("ReportID", txt_BillReportNo.Text)
                    cmdDelete.ExecuteNonQuery()
                    ' Clear controls...
                    PPD2.Document = PD2
                    PPD2.ShowDialog()
                    PD2.Print()
                    ClearAlReportControls()
                End Using
            Catch ex As Exception
                ' Display error message...
                MsgBox("Error: " & ex.Message)
            Finally
                If con.State = ConnectionState.Open Then
                    con.Close()
                End If
            End Try
        End If

        Populatedvg(con, "ReportData", DataGridView5)
    End Sub

    Private Sub Guna2GradientButton15_Click(sender As Object, e As EventArgs) Handles Guna2GradientButton15.Click
        ClearAlReportControls()

    End Sub

    Private Sub ComboBox7_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox7.SelectedIndexChanged
        Try
            con.Open()

            Dim Colmntype As String = ""
            Dim tablename As String = "ReportData"

            ' Determine the column based on ComboBox selection
            If ComboBox7.SelectedIndex = 0 Then
                Colmntype = "PatientName"
            ElseIf ComboBox7.SelectedIndex = 1 Then
                Colmntype = "DoctorName"
            ElseIf ComboBox7.SelectedIndex = 2 Then
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

            TextBox17.AutoCompleteCustomSource = ElementsToSuggest
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
        FillPersonalDetailsInFeilds()
        DTP_DOBPtn.MaxDate = Today.Date
        DTP_StffDob.MaxDate = Today.Date
        Populatedvg(con, "CustomerData", DataGridView1)
        Populatedvg(con, "StaffData", DataGridView2)
        Populatedvg(con, "TestData", DataGridView3)
        Populatedvg(con, "AppointMentData", DataGridView6)
        Populatedvg(con, "ReportData", DataGridView4)
        Populatedvg(con, "ReportData", DataGridView5)
        FillcomboBox(con, Txt_AppntTestName, "TestData", "TestName")
        FillSelectedColumnIncomboBox(con, Txt_AppntDoctorName, "StaffData", "Name", "Desgnation", "Doctor")
        AutoCompleteSearchBoxForTextBoxesTypeINt(con, Txt_AppntPtnName, "PatientID", "CustomerData", 0)
        AutoCompleteSearchBoxForTextBoxesTypeString(con, Txt_TestNameSearch, "TestName", "TestData", 0)
        AutoCompleteSearchBoxForTextBoxesTypeString(con, Txt_TestNameSearch, "TestName", "TestData", 0)
        AutoCompleteSearchBoxForTextBoxesTypeINt(con, Txt_Ptnid, "PatientID", "CustomerData", 0)
        AutoCompleteSearchBoxForTextBoxesTypeINt(con, Txt_StaffId, "StaffId", "StaffData", 0)
        AutoCompleteSearchBoxForTextBoxesTypeINt(con, TextBox36, "AppointMentID", "AppointMentData", 0)
        StaffId = TextBox1.Text
    End Sub

    Private Sub Guna2GradientButton26_Click(sender As Object, e As EventArgs) Handles Guna2GradientButton26.Click
        LoginForm.Show()
        Me.Hide()
    End Sub

    Private Sub Txt_PtnPhnNo_TextChanged(sender As Object, e As EventArgs) Handles Txt_PtnPhnNo.TextChanged
        ValidNum(Txt_PtnPhnNo)
    End Sub

    Private Sub Txt_StaffPhnNo_TextChanged(sender As Object, e As EventArgs) Handles Txt_StaffPhnNo.TextChanged
        ValidNum(Txt_StaffPhnNo)
    End Sub


    Private Sub Txt_StaffPhnNo_KeyPress(sender As Object, e As KeyPressEventArgs) Handles Txt_PtnPhnNo.KeyPress
        ' Check if the entered key is a number or a control key (like backspace)
        If Not Char.IsControl(e.KeyChar) AndAlso Not Char.IsDigit(e.KeyChar) Then
            ' If it's not a number or control key, suppress the key press
            e.Handled = True
        End If
    End Sub

    Private Sub Txt_PtnPhnNo_KeyPress(sender As Object, e As KeyPressEventArgs) Handles Txt_PtnPhnNo.KeyPress
        ' Check if the entered key is a number or a control key (like backspace)
        If Not Char.IsControl(e.KeyChar) AndAlso Not Char.IsDigit(e.KeyChar) Then
            ' If it's not a number or control key, suppress the key press
            e.Handled = True
        End If
    End Sub

    Private Sub Txt_PtnPhnNo_Leave(sender As Object, e As EventArgs) Handles Txt_PtnPhnNo.Leave

    End Sub

    Private Sub Txt_PtnPhnNo_Validating(sender As Object, e As System.ComponentModel.CancelEventArgs) Handles Txt_PtnPhnNo.Validating
        ' Check if the entered value in Txt_PtnPhnNo is a valid phone number
        Dim phoneNumber As String = Txt_PtnPhnNo.Text

        ' Trim any leading or trailing spaces
        phoneNumber = phoneNumber.Trim()

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
    Private Sub Txt_AppntPtnPhnNumber_Validating(sender As Object, e As System.ComponentModel.CancelEventArgs) Handles Txt_AppntPtnPhnNumber.Validating
        ' Check if the entered value in Txt_PtnPhnNo is a valid phone number
        Dim phoneNumber As String = Txt_AppntPtnPhnNumber.Text

        ' Trim any leading or trailing spaces
        phoneNumber = phoneNumber.Trim()

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

    Private Sub Txt_StaffPhnNo_Validating(sender As Object, e As System.ComponentModel.CancelEventArgs) Handles Txt_PtnPhnNo.Validating
        ' Check if the entered value in Txt_PtnPhnNo is a valid phone number
        Dim phoneNumber As String = Txt_StaffPhnNo.Text

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
