<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeBehind="CanadaWideWages.aspx.cs" Inherits="Wages._Default" %>
    
<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent" >
    <script type="text/javascript" src="https://www.google.com/jsapi"></script>
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
 <asp:Label ID="Info" runat="server" Text="Select Year and Province from drop down below:"></asp:Label>
<br/>
<br />
<asp:DropDownList ID="Year1" runat="server"/>&nbsp; <asp:DropDownList ID="Province1" runat="server"/>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
<asp:DropDownList ID="Year2" runat="server"/>&nbsp; <asp:DropDownList ID="Province2" runat="server"/>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
    <asp:Button ID="Compare" runat="server" Text="Compare" 
        onclick="Compare_Click" />
<br />
<br />
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <br />
            <asp:Label ID="Result1" runat="server" Text=""></asp:Label>
            <br />
            <br />
            <asp:Label ID="Result2" runat="server" Text=""></asp:Label>
            <br />
            <br />
            <asp:Label ID="Percentage" runat="server" Text=""></asp:Label>
        </ContentTemplate>
    </asp:UpdatePanel>
    <br />
    <br />
    <br />
    <br />
<br />
<input type="hidden" id="YearJson" runat="server" />
<input type="hidden" id="ProvinceJson" runat="server" />
<div id="chart_container" style="width: 900px; height: 400px;"></div>
<script type="text/javascript">
       google.load("visualization", "1", {packages:["corechart"]});
    google.setOnLoadCallback(drawChart);
        function drawChart() {
        Year = document.getElementById('<%= YearJson.ClientID %>');
        YearWiseJson = [Year.value]; 
        var data = google.visualization.arrayToDataTable(eval("[" + Year.value + "]"));
                  var view = new google.visualization.DataView(data);


                  var options = {
                      title: "Year wise average hourly wages in Canada wide provinces [1965 - 2013]",
                      width: 900,
                      height: 400,
                      bar: { groupWidth: "95%" },
                      legend: { position: "none" },
                      isStacked: true
                  };
                  var chart = new google.visualization.ColumnChart(document.getElementById("chart_container"));
                  chart.draw(view, options);
            
     }
</script>


</asp:Content>
