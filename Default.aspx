<%@ Page Async="true" Language="C#" Inherits="truck_manifest.Default" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Strict//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd">
<html>
<head runat="server">
	<title>Default</title>
</head>
<body>
   

<form id="form1" runat="server">
    <asp:Calendar id="Calendar1" OnDayRender="Calendar1_DayRender" OnSelectionChanged="Calendar1_SelectionChanged" runat="server"></asp:Calendar>
<p>
<asp:Label id="Label1" runat="server"/></p>
 <p>
<asp:Label id="Label2" runat="server"/>
</p>

<asp:PlaceHolder ID="PlaceHolder1" runat="server"></asp:PlaceHolder>
<asp:PlaceHolder ID="PlaceHolder2" runat="server"></asp:PlaceHolder>
<p>
Manifest Report - 
	From: <asp:DropDownList id="dropdownlistFrom" runat="server">
      </asp:DropDownList>
     To: <asp:DropDownList id="dropdownlistTo" runat="server">
      </asp:DropDownList>

      <asp:LinkButton Text="Create Report" OnCommand="LinkButton_Command" CommandName="Report" CommandArgument="Manifest" runat="server" />
      </p>
      <p>
<asp:LinkButton Text="Dispatch Report" OnCommand="LinkButton_Command" CommandName="Report" CommandArgument="Dispatch" runat="server" /></p><p>
<asp:LinkButton Text="Dispatch Report - Alternative Products" OnCommand="LinkButton_Command" CommandName="Report" CommandArgument="AltDispatch" runat="server" /></p><p>
<asp:LinkButton Text="Distribution - Alternative Products" OnCommand="LinkButton_Command" CommandName="AltDist" CommandArgument="AltDistribution" runat="server" /></p><p>
<asp:LinkButton Text="Pallet Report" OnCommand="LinkButton_Command" CommandName="Report" CommandArgument="Pallet" runat="server" /></p><p>
<asp:LinkButton Text="Pallet Sheet Report" OnCommand="LinkButton_Command" CommandName="Report" CommandArgument="PalletSheet" runat="server" /></p><p>
<asp:LinkButton Text="Depot Report" OnCommand="LinkButton_Command" CommandName="Report" CommandArgument="Depot" runat="server" /></p><p>
<asp:LinkButton Text="Truck Manifest Export Debug Report" OnCommand="LinkButton_Command" CommandName="Report" CommandArgument="Debug" runat="server" /></p>

</form>

</body>
</html>
