<font face='Century Gothic'>
<table align='center'><tr><td align='center'>Welcome to</td></tr><tr><td align='center'><img width='200px' src='http://i1114.photobucket.com/albums/k536/sandualbu/logo.png' /></td></tr><tr><td align='center'><a href='http://www.youtube.com/watch?feature=player_embedded&v=FWIvsc0JZx8' target='_blank'><img src='http://img.youtube.com/vi/FWIvsc0JZx8/0.jpg' width='425' height=344 /></a></td></tr></table>
<br />
<div>KineSis is a project designed to be a help in presenting documents. Using Microsoft Kinect, it's easy to control presentations using your body gestures instead of remote controls, keyboards or any other electronic devices.</div>

<div><h2><font color='#50308F'>I. Introduction</font></h2></div>

<div>The basic presentation of KineSis is shown in the following pictures. All elements will be explained below.</div>

<table align='center' border='0'>
<tr>
<td><img src='http://i1114.photobucket.com/albums/k536/sandualbu/scr1.png' /></td>
</tr>
</table>

<br />
<table align='center' border='0'>
<tr>
<td><img src='http://i1114.photobucket.com/albums/k536/sandualbu/scr2.png' /></td>
<td><img src='http://i1114.photobucket.com/albums/k536/sandualbu/scr3.png' /></td>
</tr>
</table>

<div>As you can see, there are 2 screens:</div>

<div><font color='#50308F'>User Screen</font><b>- this screen is presented to the user and displays a real-time skeleton whom actions are leading the presentation flow. The current presentation is displayed to the user in the background. The screen is preferable to be a laptop or a desktop monitor, oriented towards the presenter (user).</div></b>

<div><font color='#2CACE3'>Presentation Screen</font><b>- this screen is presented to the public (audience). It displays only the presentation. In an ideal situation, this will be presented on a projector screen or a big TV.</div></b>

<div>The presentation content from the two screens is the same. So, the screens are synchronized.</div>

<div>The user screen has several elements which needs explanation:</div>

<div><font color='red'>User</font><b>- This is the real-time rendered skeleton of the user. As you can see, it shows the most important parts of a body: head, hands, feet. The skeleton actions basically control the presentation</div></b>

<div><font color='red'>Right Hand Touch Distance (cm)/ Left Hand Touch Distance (cm)</font><b>- They display the distance of the right, respectively left hand distance from head. The distance is represented in centimeters and it works correctly when the user is oriented to Kinect sensor. Over a configured distance (referred to</b><i>Touch Distance</i><b>with a default of 45), a hand is considered</b><i>active</i><b>or</b><i>selected</i><b>. A hand remain active until the distance decreses under another configured distance (referred to</b><i>Untouch Distance</i><b>with a default of 30). When a hand is active, the equvalent indicator changes it's background color (in the image, the left hand is active, and the top-left circle has a purple background)</div></b>

<div><font color='red'>Active Hand Pointer</font><b>- Is a moving circle, representing the position of the</b><i>first selected hand</i><b>. It's purpose is to help the user in menu navigation.</div></b>

<div><font color='red'>Menu</font><b>- Appears once a hand becomes active, in the initial position of the hand. It represents the current menu through an image. Below is the list of KineSis main menus:</div></b>

<table cellpadding='0' border='1' cellspacing='0'>
<tr><td width='64px'><img src='http://i1114.photobucket.com/albums/k536/sandualbu/main.png' alt='main' /></td><td><b>Main</b> menu. You can call it also Home or Root menu.</td></tr>
<tr><td width='64px'><img src='http://i1114.photobucket.com/albums/k536/sandualbu/shapes.png' alt='shapes' /></td><td><b>Shapes</b> menu. You can call it also Charts menu.</td></tr>
<tr><td width='64px'><img src='http://i1114.photobucket.com/albums/k536/sandualbu/pages.png' alt='pages' /></td><td><b>Pages</b> menu. You can call it also Presentation menu.</td></tr>
<tr><td width='64px'><img src='http://i1114.photobucket.com/albums/k536/sandualbu/paint.png' alt='paint' /></td><td><b>Paint</b> menu. You can call it also Draw menu.</td></tr>
</table>

<div><font color='red'>Submenus</font><b>- Each menu has a list of submenus, or action menus. The user can select one of those submenus and the selected submenu will become the main menu, with its own submenus. Abstracting the few exceptions which will be explained later, each non-home menu has a "back" submenu. The back submenu returns to the parent menu.</div></b>

<div>The menus and submenus are also called <b><i>groups</i></b>, respectively <b><i>subgroups</i></b>. The KineSis groups structure is a tree structure, which will be explained in the next sections.</div>

<div><h2><font color='#50308F'>II. Selecting a submenu (subgroup)</font></h2></div>
<div>The user is the controller. His hands are the pointers. The movement of his hands controls the menu. The menu controls the document.</div>

<div>In order to interact with the menu, one hand must be active. When a hand becomes active, the menu is expanded, centered in the position where the hand got active. So, while the hand is active, the menu doesn't move. It can just change its content. Changing the content can be made only by selecting a submenu.</div>

<div>Considering an active hand and an expanded menu, the selection of a submenu can be performed in 2 steps:</div>

<ul><li>1. Drag the hand over the submenu you want to select</li></ul>

<ul><li>2. Drag the hand back to center menu</li></ul>

<div>The picture will show the movement graphically. Consider you are in the main group and you want to select the Shapes group, you have to move your hand from the center (Home) circle to the Shapes circle and return back to Main. When you return back to Home, the Shapes will become the main menu, in the center and will have its own submenus.</div>

<table align='center' border='0'><tr><td><img src='http://i1114.photobucket.com/albums/k536/sandualbu/submenu_select-1.jpg' alt='submenu select' /></td></tr></table>



<div><h2><font color='#50308F'>III. Menu Groups Tree</font></h2></div>
<div>In KineSis, the menus are organised as a tree of menus and submenus (groups and subgroups). Except the Home Group, each group has a parent. Also, a group can have more children, each child can be also a <b>Group (G)</b> or a <b>Leaf (L)</b>. A leaf is only an action, like "Up", "Down", "Left" or "Right".</div>
Next, the groups tree:<br>
<br>
<table cellpadding='0px' border='0px' align='center' cellspacing='0px' valign='center'>
<tr>
<td align='center'>Level 1 Group</td>
<td align='center'>Level 2 Group</td>
<td align='center'>Level 3 Group</td>
<td align='center'>Level 4 Group</td>
<td align='center'>Alternative Group</td>
</tr>
<tr>
<td>
<table cellpadding='0' border='1' align='left' cellspacing='0'>
<tr>
<td width='64px'><img src='http://i1114.photobucket.com/albums/k536/sandualbu/main.png' /></td>
<td width='160px' align='center'>Home (G)</td>
</tr>
</table>
</td>
<td></td>
<td></td>
<td></td>
<td></td>
</tr>

<tr>
<td><div><img src='http://i1114.photobucket.com/albums/k536/sandualbu/t_line.png' align='right' /></div></td>
<td>
<table cellpadding='0' border='1' align='left' cellspacing='0'>
<tr>
<td width='64px'><img src='http://i1114.photobucket.com/albums/k536/sandualbu/pointer.png' /></td>
<td width='160px' align='center'>Pointer (G)</td>
</tr>
</table>
</td>
<td></td>
<td></td>
<td></td>
</tr>

<tr>
<td><div><img src='http://i1114.photobucket.com/albums/k536/sandualbu/i_line.png' align='right' /></div></td>
<td><div><img src='http://i1114.photobucket.com/albums/k536/sandualbu/l_line.png' align='right' /></div></td>
<td align='center'>-</td>
<td align='center'>-</td>
<td>
<table cellpadding='0' border='1' align='left' cellspacing='0'>
<tr>
<td width='64px'><img src='http://i1114.photobucket.com/albums/k536/sandualbu/main_s.png' /></td>
<td width='160px' align='center'><i>back to</i> Home (G)</td>
</tr>
</table>
</td>
</tr>


<tr>
<td><div><img src='http://i1114.photobucket.com/albums/k536/sandualbu/t_line.png' align='right' /></div></td>
<td>
<table cellpadding='0' border='1' align='left' cellspacing='0'>
<tr>
<td width='64px'><img src='http://i1114.photobucket.com/albums/k536/sandualbu/shapes.png' /></td>
<td width='160px' align='center'>Shapes (G)</td>
</tr>
</table>
</td>
<td></td>
<td></td>
<td></td>
</tr>

<tr>
<td><div><img src='http://i1114.photobucket.com/albums/k536/sandualbu/i_line.png' align='right' /></div></td>
<td><div><img src='http://i1114.photobucket.com/albums/k536/sandualbu/t_line.png' align='right' /></div></td>
<td>
<table cellpadding='0' border='1' align='left' cellspacing='0'>
<tr>
<td width='64px'><img src='http://i1114.photobucket.com/albums/k536/sandualbu/main_s.png' /></td>
<td width='160px' align='center'><i>back to</i> Home (G)</td>
</tr>
</table>
</td>
<td></td>
<td></td>
</tr>

<tr>
<td><div><img src='http://i1114.photobucket.com/albums/k536/sandualbu/i_line.png' align='right' /></div></td>
<td><div><img src='http://i1114.photobucket.com/albums/k536/sandualbu/t_line.png' align='right' /></div></td>
<td>
<table cellpadding='0' border='1' align='left' cellspacing='0'>
<tr>
<td width='64px'><img src='http://i1114.photobucket.com/albums/k536/sandualbu/select.png' /></td>
<td width='160px' align='center'>Select (G)</td>
</tr>
</table>
</td>
<td></td>
<td></td>
</tr>

<tr>
<td><div><img src='http://i1114.photobucket.com/albums/k536/sandualbu/i_line.png' align='right' /></div></td>
<td><div><img src='http://i1114.photobucket.com/albums/k536/sandualbu/i_line.png' align='right' /></div></td>
<td><div><img src='http://i1114.photobucket.com/albums/k536/sandualbu/t_line.png' align='right' /></div></td>
<td>
<table cellpadding='0' border='1' align='left' cellspacing='0'>
<tr>
<td width='64px'><img src='http://i1114.photobucket.com/albums/k536/sandualbu/left.png' /></td>
<td width='160px' align='center'>Previous (L)</td>
</tr>
</table>
</td>
<td>
<table cellpadding='0' border='1' align='left' cellspacing='0'>
<tr>
<td width='64px'><img src='http://i1114.photobucket.com/albums/k536/sandualbu/shapes_s.png' /></td>
<td width='160px' align='center'><i>back to</i> Shapes (G)</td>
</tr>
</table>
</td>
</tr>

<tr>
<td><div><img src='http://i1114.photobucket.com/albums/k536/sandualbu/i_line.png' align='right' /></div></td>
<td><div><img src='http://i1114.photobucket.com/albums/k536/sandualbu/i_line.png' align='right' /></div></td>
<td><div><img src='http://i1114.photobucket.com/albums/k536/sandualbu/t_line.png' align='right' /></div></td>
<td>
<table cellpadding='0' border='1' align='left' cellspacing='0'>
<tr>
<td width='64px'><img src='http://i1114.photobucket.com/albums/k536/sandualbu/select_s.png' /></td>
<td width='160px' align='center'>Select (L)</td>
</tr>
</table>
</td>
<td></td>
</tr>
<tr>
<td><div><img src='http://i1114.photobucket.com/albums/k536/sandualbu/i_line.png' align='right' /></div></td>
<td><div><img src='http://i1114.photobucket.com/albums/k536/sandualbu/i_line.png' align='right' /></div></td>
<td><div><img src='http://i1114.photobucket.com/albums/k536/sandualbu/l_line.png' align='right' /></div></td>
<td>
<table cellpadding='0' border='1' align='left' cellspacing='0'>
<tr>
<td width='64px'><img src='http://i1114.photobucket.com/albums/k536/sandualbu/right.png' /></td>
<td width='160px' align='center'>Next (L)</td>
</tr>
</table>
</td>
<td></td>
</tr>

<tr>
<td><div><img src='http://i1114.photobucket.com/albums/k536/sandualbu/i_line.png' align='right' /></div></td>
<td><div><img src='http://i1114.photobucket.com/albums/k536/sandualbu/t_line.png' align='right' /></div></td>
<td>
<table cellpadding='0' border='1' align='left' cellspacing='0'>
<tr>
<td width='64px'><img src='http://i1114.photobucket.com/albums/k536/sandualbu/close.png' /></td>
<td width='160px' align='center'>Close (L)</td>
</tr>
</table>
</td>
<td></td>
<td></td>
</tr>

<tr>
<td><div><img src='http://i1114.photobucket.com/albums/k536/sandualbu/i_line.png' align='right' /></div></td>
<td><div><img src='http://i1114.photobucket.com/albums/k536/sandualbu/l_line.png' align='right' /></div></td>
<td>
<table cellpadding='0' border='1' align='left' cellspacing='0'>
<tr>
<td width='64px'><img src='http://i1114.photobucket.com/albums/k536/sandualbu/rotate.png' /></td>
<td width='160px' align='center'>Rotate (G)</td>
</tr>
</table>
</td>
<td></td>
<td></td>
</tr>

<tr>
<td><div><img src='http://i1114.photobucket.com/albums/k536/sandualbu/i_line.png' align='right' /></div></td>
<td></td>
<td><div><img src='http://i1114.photobucket.com/albums/k536/sandualbu/t_line.png' align='right' /></div></td>
<td>
<table cellpadding='0' border='1' align='left' cellspacing='0'>
<tr>
<td width='64px'><img src='http://i1114.photobucket.com/albums/k536/sandualbu/left.png' /></td>
<td width='160px' align='center'>Left (L)</td>
</tr>
</table>
</td>
<td>
<table cellpadding='0' border='1' align='left' cellspacing='0'>
<tr>
<td width='64px'><img src='http://i1114.photobucket.com/albums/k536/sandualbu/shapes_s.png' /></td>
<td width='160px' align='center'><i>back to</i> Shapes (G)</td>
</tr>
</table>
</td>
</tr>

<tr>
<td><div><img src='http://i1114.photobucket.com/albums/k536/sandualbu/i_line.png' align='right' /></div></td>
<td></td>
<td><div><img src='http://i1114.photobucket.com/albums/k536/sandualbu/t_line.png' align='right' /></div></td>
<td>
<table cellpadding='0' border='1' align='left' cellspacing='0'>
<tr>
<td width='64px'><img src='http://i1114.photobucket.com/albums/k536/sandualbu/up.png' /></td>
<td width='160px' align='center'>Up (L)</td>
</tr>
</table>
</td>
<td></td>
</tr>
<tr>
<td><div><img src='http://i1114.photobucket.com/albums/k536/sandualbu/i_line.png' align='right' /></div></td>
<td></td>
<td><div><img src='http://i1114.photobucket.com/albums/k536/sandualbu/t_line.png' align='right' /></div></td>
<td>
<table cellpadding='0' border='1' align='left' cellspacing='0'>
<tr>
<td width='64px'><img src='http://i1114.photobucket.com/albums/k536/sandualbu/right.png' /></td>
<td width='160px' align='center'>Right (L)</td>
</tr>
</table>
</td>
<td></td>
</tr>
<tr>
<td><div><img src='http://i1114.photobucket.com/albums/k536/sandualbu/i_line.png' align='right' /></div></td>
<td></td>
<td><div><img src='http://i1114.photobucket.com/albums/k536/sandualbu/l_line.png' align='right' /></div></td>
<td>
<table cellpadding='0' border='1' align='left' cellspacing='0'>
<tr>
<td width='64px'><img src='http://i1114.photobucket.com/albums/k536/sandualbu/down.png' /></td>
<td width='160px' align='center'>Down (L)</td>
</tr>
</table>
</td>
<td></td>
</tr>


<tr>
<td><div><img src='http://i1114.photobucket.com/albums/k536/sandualbu/t_line.png' align='right' /></div></td>
<td>
<table cellpadding='0' border='1' align='left' cellspacing='0'>
<tr>
<td width='64px'><img src='http://i1114.photobucket.com/albums/k536/sandualbu/pages.png' /></td>
<td width='160px' align='center'>Pages (G)</td>
</tr>
</table>
</td>
<td></td>
<td></td>
<td></td>
</tr>

<tr>
<td><div><img src='http://i1114.photobucket.com/albums/k536/sandualbu/i_line.png' align='right' /></div></td>
<td><div><img src='http://i1114.photobucket.com/albums/k536/sandualbu/t_line.png' align='right' /></div></td>
<td>
<table cellpadding='0' border='1' align='left' cellspacing='0'>
<tr>
<td width='64px'><img src='http://i1114.photobucket.com/albums/k536/sandualbu/main_s.png' /></td>
<td width='160px' align='center'><i>back to</i> Home (G)</td>
</tr>
</table>
</td>
<td></td>
<td></td>
</tr>

<tr>
<td><div><img src='http://i1114.photobucket.com/albums/k536/sandualbu/i_line.png' align='right' /></div></td>
<td><div><img src='http://i1114.photobucket.com/albums/k536/sandualbu/t_line.png' align='right' /></div></td>
<td>
<table cellpadding='0' border='1' align='left' cellspacing='0'>
<tr>
<td width='64px'><img src='http://i1114.photobucket.com/albums/k536/sandualbu/select.png' /></td>
<td width='160px' align='center'>Select (G)</td>
</tr>
</table>
</td>
<td></td>
<td></td>
</tr>

<tr>
<td><div><img src='http://i1114.photobucket.com/albums/k536/sandualbu/i_line.png' align='right' /></div></td>
<td><div><img src='http://i1114.photobucket.com/albums/k536/sandualbu/i_line.png' align='right' /></div></td>
<td><div><img src='http://i1114.photobucket.com/albums/k536/sandualbu/t_line.png' align='right' /></div></td>
<td>
<table cellpadding='0' border='1' align='left' cellspacing='0'>
<tr>
<td width='64px'><img src='http://i1114.photobucket.com/albums/k536/sandualbu/left.png' /></td>
<td width='160px' align='center'>Previous (L)</td>
</tr>
</table>
</td>
<td>
<table cellpadding='0' border='1' align='left' cellspacing='0'>
<tr>
<td width='64px'><img src='http://i1114.photobucket.com/albums/k536/sandualbu/pages_s.png' /></td>
<td width='160px' align='center'><i>back to</i> Pages (G)</td>
</tr>
</table>
</td>
</tr>

<tr>
<td><div><img src='http://i1114.photobucket.com/albums/k536/sandualbu/i_line.png' align='right' /></div></td>
<td><div><img src='http://i1114.photobucket.com/albums/k536/sandualbu/i_line.png' align='right' /></div></td>
<td><div><img src='http://i1114.photobucket.com/albums/k536/sandualbu/t_line.png' align='right' /></div></td>
<td>
<table cellpadding='0' border='1' align='left' cellspacing='0'>
<tr>
<td width='64px'><img src='http://i1114.photobucket.com/albums/k536/sandualbu/select_s.png' /></td>
<td width='160px' align='center'>Select (L)</td>
</tr>
</table>
</td>
<td></td>
</tr>
<tr>
<td><div><img src='http://i1114.photobucket.com/albums/k536/sandualbu/i_line.png' align='right' /></div></td>
<td><div><img src='http://i1114.photobucket.com/albums/k536/sandualbu/i_line.png' align='right' /></div></td>
<td><div><img src='http://i1114.photobucket.com/albums/k536/sandualbu/l_line.png' align='right' /></div></td>
<td>
<table cellpadding='0' border='1' align='left' cellspacing='0'>
<tr>
<td width='64px'><img src='http://i1114.photobucket.com/albums/k536/sandualbu/right.png' /></td>
<td width='160px' align='center'>Next (L)</td>
</tr>
</table>
</td>
<td></td>
</tr>

<tr>
<td><div><img src='http://i1114.photobucket.com/albums/k536/sandualbu/i_line.png' align='right' /></div></td>
<td><div><img src='http://i1114.photobucket.com/albums/k536/sandualbu/t_line.png' align='right' /></div></td>
<td>
<table cellpadding='0' border='1' align='left' cellspacing='0'>
<tr>
<td width='64px'><img src='http://i1114.photobucket.com/albums/k536/sandualbu/navigate.png' /></td>
<td width='160px' align='center'>Navigate (G)</td>
</tr>
</table>
</td>
<td></td>
<td></td>
</tr>

<tr>
<td><div><img src='http://i1114.photobucket.com/albums/k536/sandualbu/i_line.png' align='right' /></div></td>
<td><div><img src='http://i1114.photobucket.com/albums/k536/sandualbu/i_line.png' align='right' /></div></td>
<td><div><img src='http://i1114.photobucket.com/albums/k536/sandualbu/t_line.png' align='right' /></div></td>
<td>
<table cellpadding='0' border='1' align='left' cellspacing='0'>
<tr>
<td width='64px'><img src='http://i1114.photobucket.com/albums/k536/sandualbu/left.png' /></td>
<td width='160px' align='center'>Previous (L)</td>
</tr>
</table>
</td>
<td>
<table cellpadding='0' border='1' align='left' cellspacing='0'>
<tr>
<td width='64px'><img src='http://i1114.photobucket.com/albums/k536/sandualbu/pages_s.png' /></td>
<td width='160px' align='center'><i>back to</i> Pages (G)</td>
</tr>
</table>
</td>
</tr>

<tr>
<td><div><img src='http://i1114.photobucket.com/albums/k536/sandualbu/i_line.png' align='right' /></div></td>
<td><div><img src='http://i1114.photobucket.com/albums/k536/sandualbu/i_line.png' align='right' /></div></td>
<td><div><img src='http://i1114.photobucket.com/albums/k536/sandualbu/l_line.png' align='right' /></div></td>
<td>
<table cellpadding='0' border='1' align='left' cellspacing='0'>
<tr>
<td width='64px'><img src='http://i1114.photobucket.com/albums/k536/sandualbu/right.png' /></td>
<td width='160px' align='center'>Next (L)</td>
</tr>
</table>
</td>
<td></td>
</tr>

<tr>
<td><div><img src='http://i1114.photobucket.com/albums/k536/sandualbu/i_line.png' align='right' /></div></td>
<td><div><img src='http://i1114.photobucket.com/albums/k536/sandualbu/l_line.png' align='right' /></div></td>
<td>
<table cellpadding='0' border='1' align='left' cellspacing='0'>
<tr>
<td width='64px'><img src='http://i1114.photobucket.com/albums/k536/sandualbu/scroll.png' /></td>
<td width='160px' align='center'>Scroll (G)</td>
</tr>
</table>
</td>
<td></td>
<td></td>
</tr>

<tr>
<td><div><img src='http://i1114.photobucket.com/albums/k536/sandualbu/i_line.png' align='right' /></div></td>
<td></td>
<td><div><img src='http://i1114.photobucket.com/albums/k536/sandualbu/t_line.png' align='right' /></div></td>
<td>
<table cellpadding='0' border='1' align='left' cellspacing='0'>
<tr>
<td width='64px'><img src='http://i1114.photobucket.com/albums/k536/sandualbu/left.png' /></td>
<td width='160px' align='center'>Left (L)</td>
</tr>
</table>
</td>
<td>
<table cellpadding='0' border='1' align='left' cellspacing='0'>
<tr>
<td width='64px'><img src='http://i1114.photobucket.com/albums/k536/sandualbu/pages_s.png' /></td>
<td width='160px' align='center'><i>back to</i> Pages (G)</td>
</tr>
</table>
</td>
</tr>

<tr>
<td><div><img src='http://i1114.photobucket.com/albums/k536/sandualbu/i_line.png' align='right' /></div></td>
<td></td>
<td><div><img src='http://i1114.photobucket.com/albums/k536/sandualbu/t_line.png' align='right' /></div></td>
<td>
<table cellpadding='0' border='1' align='left' cellspacing='0'>
<tr>
<td width='64px'><img src='http://i1114.photobucket.com/albums/k536/sandualbu/up.png' /></td>
<td width='160px' align='center'>Up (L)</td>
</tr>
</table>
</td>
<td>
<table cellpadding='0' border='1' align='left' cellspacing='0'>
<tr>
<td width='64px'><img src='http://i1114.photobucket.com/albums/k536/sandualbu/zoom_in.png' /></td>
<td width='160px' align='center'>Zoom In (L)</td>
</tr>
</table>
</td>
</tr>
<tr>
<td><div><img src='http://i1114.photobucket.com/albums/k536/sandualbu/i_line.png' align='right' /></div></td>
<td></td>
<td><div><img src='http://i1114.photobucket.com/albums/k536/sandualbu/t_line.png' align='right' /></div></td>
<td>
<table cellpadding='0' border='1' align='left' cellspacing='0'>
<tr>
<td width='64px'><img src='http://i1114.photobucket.com/albums/k536/sandualbu/right.png' /></td>
<td width='160px' align='center'>Right (L)</td>
</tr>
</table>
</td>
<td>
<table cellpadding='0' border='1' align='left' cellspacing='0'>
<tr>
<td width='64px'><img src='http://i1114.photobucket.com/albums/k536/sandualbu/fit.png' /></td>
<td width='160px' align='center'>Zoom Fit (L)</td>
</tr>
</table>
</td>
</tr>
<tr>
<td><div><img src='http://i1114.photobucket.com/albums/k536/sandualbu/i_line.png' align='right' /></div></td>
<td></td>
<td><div><img src='http://i1114.photobucket.com/albums/k536/sandualbu/l_line.png' align='right' /></div></td>
<td>
<table cellpadding='0' border='1' align='left' cellspacing='0'>
<tr>
<td width='64px'><img src='http://i1114.photobucket.com/albums/k536/sandualbu/down.png' /></td>
<td width='160px' align='center'>Down (L)</td>
</tr>
</table>
</td>
<td>
<table cellpadding='0' border='1' align='left' cellspacing='0'>
<tr>
<td width='64px'><img src='http://i1114.photobucket.com/albums/k536/sandualbu/zoom_out.png' /></td>
<td width='160px' align='center'>Zoom Out (L)</td>
</tr>
</table>
</td>
</tr>


<tr>
<td><div><img src='http://i1114.photobucket.com/albums/k536/sandualbu/l_line.png' align='right' /></div></td>
<td>
<table cellpadding='0' border='1' align='left' cellspacing='0'>
<tr>
<td width='64px'><img src='http://i1114.photobucket.com/albums/k536/sandualbu/paint.png' /></td>
<td width='160px' align='center'>Paint (G)</td>
</tr>
</table>
</td>
<td></td>
<td></td>
<td></td>
</tr>

<tr>
<td></td>
<td><div><img src='http://i1114.photobucket.com/albums/k536/sandualbu/t_line.png' align='right' /></div></td>
<td>
<table cellpadding='0' border='1' align='left' cellspacing='0'>
<tr>
<td width='64px'><img src='http://i1114.photobucket.com/albums/k536/sandualbu/main_s.png' /></td>
<td width='160px' align='center'><i>back to</i> Home (G)<br /><i><code>[</code>when not painting<code>]</code></i></td>
</tr>
</table>
</td>
<td align='center'>-</td>
<td>
<table cellpadding='0' border='1' align='left' cellspacing='0'>
<tr>
<td width='64px'><img src='http://i1114.photobucket.com/albums/k536/sandualbu/main_s.png' /></td>
<td width='160px' align='center'><i>back to</i> Home (G)</td>
</tr>
</table>
</td>
</tr>

<tr>
<td></td>
<td><div><img src='http://i1114.photobucket.com/albums/k536/sandualbu/t_line.png' align='right' /></div></td>
<td>
<table cellpadding='0' border='1' align='left' cellspacing='0'>
<tr>
<td width='64px'><img src='http://i1114.photobucket.com/albums/k536/sandualbu/color1.png' /></td>
<td width='160px' align='center'>Color 1 (L)<br /><i><code>[</code>when not painting<code>]</code></i></td>
</tr>
</table>
</td>
<td align='center'>-</td>
<td>
<table cellpadding='0' border='1' align='left' cellspacing='0'>
<tr>
<td width='64px'><img src='http://i1114.photobucket.com/albums/k536/sandualbu/color2.png' /></td>
<td width='160px' align='center'>Color 1/2/3 (L)</td>
</tr>
</table>
</td>
</tr>
<tr>
<td></td>
<td><div><img src='http://i1114.photobucket.com/albums/k536/sandualbu/t_line.png' align='right' /></div></td>
<td>
<table cellpadding='0' border='1' align='left' cellspacing='0'>
<tr>
<td width='64px'><img src='http://i1114.photobucket.com/albums/k536/sandualbu/clear.png' /></td>
<td width='160px' align='center'>Clear (L)<br /><i><code>[</code>when not painting<code>]</code></i></td>
</tr>
</table>
</td>
<td align='center'>-</td>
<td>
<table cellpadding='0' border='1' align='left' cellspacing='0'>
<tr>
<td width='64px'><img src='http://i1114.photobucket.com/albums/k536/sandualbu/clear.png' /></td>
<td width='160px' align='center'>Clear (L)</td>
</tr>
</table>
</td>
</tr>
<tr>
<td></td>
<td><div><img src='http://i1114.photobucket.com/albums/k536/sandualbu/l_line.png' align='right' /></div></td>
<td>
<table cellpadding='0' border='1' align='left' cellspacing='0'>
<tr>
<td width='64px'><img src='http://i1114.photobucket.com/albums/k536/sandualbu/color2.png' /></td>
<td width='160px' align='center'>Color 2 (L)<br /><i><code>[</code>when not painting<code>]</code></i></td>
</tr>
</table>
</td>
<td align='center'>-</td>
<td>
<table cellpadding='0' border='1' align='left' cellspacing='0'>
<tr>
<td width='64px'><img src='http://i1114.photobucket.com/albums/k536/sandualbu/color1.png' /></td>
<td width='160px' align='center'>Color 1/2/3 (L)</td>
</tr>
</table>
</td>
</tr>
</table>
<br />
<div>KineSis has its menu structured in 4 levels. First level is the main Menu, or the root menu. It has 4 children, the level 2 groups. Excepting Pointer, each level 2 group has level 3 children. Level 4 children are only leafs, so just actions, like "Up" and "Down". The Alternative groups are a kind of secondary groups. They are shown only when the user raises a hand up and acts with the other one.</div>

<div>The difference between opening a menu and opening the alternative menu it's shown in the following picture. As you can see, for opening the alternative menu, the user keeps a hand in the air. All the time the user raises a hand above his head, his skeleton is colored with primary color, instead of skeleton color. This way, the user will know he is about to act on an alternative menu (if it exists) and not on a primary menu.</div>

<table align='center' border='0'><tr><td><img src='http://i1114.photobucket.com/albums/k536/sandualbu/menu_activation_s.jpg' /></td></tr></table>
<div>The maximum number of children a group can display is 4. In case of more than 4 groups, an alternative can be used. In any case, a way back to parent is ensured. In the figure above, the scroll menu is given as example. Primary actions are scrolling left, up, right and down. There is no place for more, like back to Home, so here the secondary menu comes into action. Raising a hand up, the secondary (alternative) menu is presented to the user. The options are: turning back to Home, zooming in, locking/unlocking of zoom (at page width or 100%) and zooming out. Once you’ve activated the alternative menu, you can let the raised hand down to return to the primary menu. If the active hand is no longer active, the menu (primary or alternative) will disappear until you activate it again.</div>

<div>Now, let's see each menu/submenu in action!</div>



<div><table border='0'><tr><td width='64px'><img src='http://i1114.photobucket.com/albums/k536/sandualbu/pointer.png' /></td><td><font color='#50308F' size='4'><b>1. Pointer</b></font></td></tr></table></div>

<div>The Pointer menu is the simplest menu of KineSis. First, you have to select it from main menu. Make a hand active, drag it to the pointer icon and back to center (Home).</div>
<table align='center' border='0'><tr><td><img src='http://i1114.photobucket.com/albums/k536/sandualbu/pointer1.jpg' /></td></tr></table>
<div>After entering in menu, as long you have one hand active, you will see a colorful slinky following your hand movement. The slinky appears on both screens, user and presentation. It is useful when you want to point something, in such a way everybody will know what exactly you are talking about. You can compare the Pointer with a hand laser used by 'old school' presenters on projectors.</div>
<table align='center' border='0'><tr><td><img src='http://i1114.photobucket.com/albums/k536/sandualbu/pointer2.jpg' /></td></tr></table>
<div>To exit from Pointer menu and return back to home, raise a hand up and make the other one active, so you can see the secondary (alternative) menu. You can only get back to Home.</div>
<table align='center' border='0'><tr><td><img src='http://i1114.photobucket.com/albums/k536/sandualbu/pointer3.jpg' /></td></tr></table>

<br /><br /><br />
<div><table border='0'><tr><td width='64px'><img src='http://i1114.photobucket.com/albums/k536/sandualbu/shapes.png' /></td><td><font color='#50308F' size='4'><b>2. Shapes</b></font></td></tr></table></div>
<div>Not all menus are active anytime. For example, the Shapes menu is active only when current page of the presentation contains charts. And not any charts. KineSis supports only Microsoft Office Charts (2D and 3D).</div>
<div>For navigation between pages, you have to read the Pages menu. Let's assume you are on a page which contains charts, like in the following figure.</div>
<table align='center' border='0'><tr><td><img src='http://i1114.photobucket.com/albums/k536/sandualbu/shapes1.jpg' /></td></tr></table>
<div>After you get in Shapes menu, you have 4 options: to select a chart and make it full screen, to close an opened chart, or to rotate an opened chart.</div>
<table align='center' border='0'><tr><td><img src='http://i1114.photobucket.com/albums/k536/sandualbu/shapes2.jpg' /></td></tr></table>
<div>As you can see in the figure, no chart is opened, so you cannot close or rotate anything. So, first you have to select a chart.</div>

<br /><br />
<blockquote><div><table border='0'><tr><td><img src='http://i1114.photobucket.com/albums/k536/sandualbu/select.png' /></td><td><font color='#50308F' size='3'><b>2.1 Select a Chart</b></font></td></tr></table></div>
<div>Selection menu have 3 major elements: the 'select' submenu, the navigation submenus and the preview section. You can see them in the following figure.</div>
<table align='center' border='0'><tr><td><img src='http://i1114.photobucket.com/albums/k536/sandualbu/shapes3.jpg' /></td></tr></table>
<div>You have the possibility of searching through charts without making them full screen, so you will show the desired chart to the public only when you find the right chart. Navigation through charts can be made using the navigation menu. You can call 'next' or 'previous' chart. In the above figure, the user is calling the next chart. The movement is simple: drag the active hand in the corresponding circle and the back to the center circle. You can see at the bottom of screen the charts. The one inthe middle (also the title is displayed above) is the one you are looking now. Calling 'next' or 'previous' will change the preview section.</div>
<div>When you find the right chart and you want to show it to the public, you must drag the active hand into the 'select' circle, placed in the top of the screen, like in the following figure.</div>
<table align='center' border='0'><tr><td><img src='http://i1114.photobucket.com/albums/k536/sandualbu/shapes4.jpg' /></td></tr></table>
<div>Now the chart is full screen and visible to everybody. To act on the selected chart you have to return back to 'Shapes' menu. You can do that by activating the secondary menu of select, by raising up a hand and the other one is active.</div>
<table align='center' border='0'><tr><td><img src='http://i1114.photobucket.com/albums/k536/sandualbu/shapes5.jpg' /></td></tr></table>
<div>Now you are back to 'Shapes' menu. The 'Clear' menu is active, and if the chart is 3D, also the 'Rotate' menu is active.</div></blockquote>

<br /><br />
<blockquote><div><table border='0'><tr><td><img src='http://i1114.photobucket.com/albums/k536/sandualbu/rotate.png' /></td><td><font color='#50308F' size='3'><b>2.2 Rotate a Chart</b></font></td></tr></table></div>
<div>You can rotate a chart only if the chart is 3-dimesional and was processed previously by KineSis (More about this on Settings and Documents section). Also, the chart must be previously selected and made full screen.</div>
<div>First, you have to enter in 'Rotate' menu.</div>
<table align='center' border='0'><tr><td><img src='http://i1114.photobucket.com/albums/k536/sandualbu/shapes6.jpg' /></td></tr></table>
<div>The 'Rotate' let user rotate the charts in 4 directions - to left, up, right and down.</div>
<table align='center' border='0'><tr><td><img src='http://i1114.photobucket.com/albums/k536/sandualbu/shapes7.jpg' /></td></tr></table>
<div>To return back from the 'Rotate' menu, you must activate the secondary menu and follow your way back.</div>
<table align='center' border='0'><tr><td><img src='http://i1114.photobucket.com/albums/k536/sandualbu/shapes8.jpg' /></td></tr></table></blockquote>

<br /><br />
<blockquote><div><table border='0'><tr><td><img src='http://i1114.photobucket.com/albums/k536/sandualbu/close.png' /></td><td><font color='#50308F' size='3'><b>2.3 Close a Chart</b></font></td></tr></table></div>
<div>The only remaining thing to do is to close the opened chart. The 'Close' menu is an action menu, so it doesn't have any children. It's enought only to drag the active hand over it and back to the center (Shapes) circle and the opened chart is closed.</div>
<table align='center' border='0'><tr><td><img src='http://i1114.photobucket.com/albums/k536/sandualbu/shapes9.jpg' /></td></tr></table></blockquote>

<br /><br />
<blockquote><div><table border='0'><tr><td><img src='http://i1114.photobucket.com/albums/k536/sandualbu/main_s.png' /></td><td><font color='#50308F' size='3'><b>2.4 Go Back</b></font></td></tr></table></div>
<div>Turning back from 'Shapes' menu to Main menu can be made by selecting the 'back to Home' submenu.</div>
<table align='center' border='0'><tr><td><img src='http://i1114.photobucket.com/albums/k536/sandualbu/shapes10.jpg' /></td></tr></table></blockquote>

<br /><br /><br />
<div><table border='0'><tr><td width='64px'><img src='http://i1114.photobucket.com/albums/k536/sandualbu/pages.png' /></td><td><font color='#50308F' size='4'><b>3. Pages</b></font></td></tr></table></div>
<div>Like 'Shapes', the 'Pages' menu is not always active. You can use it only when you have a document opened. 'Pages' menu allows you to navigate through pages, zoom and scroll. To enter in 'Pages' menu, you have to select it from Main menu.</div>
<table align='center' border='0'><tr><td><img src='http://i1114.photobucket.com/albums/k536/sandualbu/pages1.jpg' /></td></tr></table>
<div>From here, you can go to 'Select Page', 'Navigate', 'Scroll and Zoom', or back to Home.</div>

<br /><br />
<blockquote><div><table border='0'><tr><td><img src='http://i1114.photobucket.com/albums/k536/sandualbu/select.png' /></td><td><font color='#50308F' size='3'><b>3.1 Select a Page</b></font></td></tr></table></div>
<div>Selecting a page is the same as selecting a chart. First, select the 'Select' menu.</div>
<table align='center' border='0'><tr><td><img src='http://i1114.photobucket.com/albums/k536/sandualbu/pages2.jpg' /></td></tr></table>
<div>The only difference here is that instead of charts, now you navigate through document pages or slides.</div>
<table align='center' border='0'><tr><td><img src='http://i1114.photobucket.com/albums/k536/sandualbu/pages3.jpg' /></td></tr></table>
<div>To return, like in 'Select Chart', activate the secondary menu and choose 'Back to Pages'.</div></blockquote>

<br /><br />
<blockquote><div><table border='0'><tr><td><img src='http://i1114.photobucket.com/albums/k536/sandualbu/navigate.png' /></td><td><font color='#50308F' size='3'><b>3.2 Navigate</b></font></td></tr></table></div>
<div>Navigation menu is a menu for intensive usage. All you can do from here is to go to the next or previous page. Because of this, it was kept as simple as possible. To enter in Navigation menu, select it from 'Pages' submenus.</div>
<table align='center' border='0'><tr><td><img src='http://i1114.photobucket.com/albums/k536/sandualbu/pages4.jpg' /></td></tr></table>
<div>Here, with active hand, you can navigate to the next or previous page. The flow is simple: drag the active hand over 'next' or 'previous' circle and then back to center circle.</div>
<table align='center' border='0'><tr><td><img src='http://i1114.photobucket.com/albums/k536/sandualbu/pages5.jpg' /></td></tr></table>
<div>To return to 'Pages' menu, activate the secondary menu by raising a hand up and make the other hand active. Then select 'Back to Pages' option.</div>
<table align='center' border='0'><tr><td><img src='http://i1114.photobucket.com/albums/k536/sandualbu/pages6.jpg' /></td></tr></table></blockquote>

<br /><br />
<blockquote><div><table border='0'><tr><td><img src='http://i1114.photobucket.com/albums/k536/sandualbu/scroll.png' /></td><td><font color='#50308F' size='3'><b>3.3 Scroll and Zoom</b></font></td></tr></table></div>
<div>Because scrolling and zooming goes hand in hand, they were put in the same menu. If you zoom in, most probably you want to scroll right after and vice versa. To enter in this menu, select it from 'Pages'.</div>
<table align='center' border='0'><tr><td><img src='http://i1114.photobucket.com/albums/k536/sandualbu/pages7.jpg' /></td></tr></table>
<div>The primary menu here is the scroll. You can scroll in 4 directions: left, up, right and down.</div>
<table align='center' border='0'><tr><td><img src='http://i1114.photobucket.com/albums/k536/sandualbu/pages9.jpg' /></td></tr></table>
<div>The secondary menu have functions for zooming in, out, locking zoom and returning back to 'Pages' menu. Locking zoom means fitting the page of the document to screen's width. When the zoom is locked, the 'Zoom In' and 'Zoom Out' actions are disabled until the zoom is unlocked. Also, on zoom lock, the scroll has no effect. The same menu activates and deactivates the zoom lock. It is represented as a closed or opened lock icon (it is the menu from right).</div>
<table align='center' border='0'><tr><td><img src='http://i1114.photobucket.com/albums/k536/sandualbu/pages8.jpg' /></td></tr></table>
<div>To return to 'Pages' menu, activate the alternative menu (here the alternative menu is also the zoom menu) and select 'Back to Pages' submenu.</div>
<table align='center' border='0'><tr><td><img src='http://i1114.photobucket.com/albums/k536/sandualbu/pages10.jpg' /></td></tr></table></blockquote>

<br /><br />
<blockquote><div><table border='0'><tr><td><img src='http://i1114.photobucket.com/albums/k536/sandualbu/main_s.png' /></td><td><font color='#50308F' size='3'><b>3.4 Go Back</b></font></td></tr></table></div>
<div>Select 'Back to Home' to return to 'Home' menu.</div>
<table align='center' border='0'><tr><td><img src='http://i1114.photobucket.com/albums/k536/sandualbu/pages11.jpg' /></td></tr></table></blockquote>

<br /><br /><br />
<div><table border='0'><tr><td width='64px'><img src='http://i1114.photobucket.com/albums/k536/sandualbu/paint.png' /></td><td><font color='#50308F' size='4'><b>4. Paint</b></font></td></tr></table></div>
<div>Paint menu is designed as a support for drawing over a presentation. You can select a color (red, green or blue), draw over presentation (everything you draw will appear on both screens), or clear the board.</div>
<div>Like Pointer, Paint is always available, so you don't need a presentation opened to use it. First, select the Paint menu from Main menu.</div>
<table align='center' border='0'><tr><td><img src='http://i1114.photobucket.com/albums/k536/sandualbu/paint1.jpg' /></td></tr></table>
<div>The Paint has 2 modes: "in paint" or "not in paint". First time you enter in paint, the mode is "not in paint". This way you can choose a color before start painting.</div>
<table align='center' border='0'><tr><td><img src='http://i1114.photobucket.com/albums/k536/sandualbu/paint2.jpg' /></td></tr></table>
<div>After you have selected a color (up and down menu), the mode becomes "in paint" and an icon is displayed on top of the screen, indicating the "in paint" mode and the selected color.</div>
<table align='center' border='0'><tr><td><img src='http://i1114.photobucket.com/albums/k536/sandualbu/paint3.jpg' /></td></tr></table>
<div>As long as you are in "in paint" mode, the active hand is used only for drawing. Exiting from this mode can be done by activating the alternative (secondary) menu, by raising a hand up and the other hand active. In paint, only one menu is available, the difference is only the mode it is displayed.</div>
<table align='center' border='0'><tr><td><img src='http://i1114.photobucket.com/albums/k536/sandualbu/paint4.jpg' /></td></tr></table>
<div>You can change the drawing color, clear the screens, or return back to Main menu. Clear menu acts like 'Close Chart' menu, but this clear all the lines you have drawn. It is active only when there is something to clear.</div>
<table align='center' border='0'><tr><td><img src='http://i1114.photobucket.com/albums/k536/sandualbu/paint5.jpg' /></td></tr></table>
<div>To go back to Main menu, select the submenu from Paint manu.</div>
<table align='center' border='0'><tr><td><img src='http://i1114.photobucket.com/albums/k536/sandualbu/paint6.jpg' /></td></tr></table>

<br /><br /><br />
<div>The following section will explain how to configure KineSis and how to deal with documents.</div>
<div><h2><font color='#50308F'>IV. Context Menu</font></h2></div>
<div>The user can interact with KineSis in 2 ways: by body gestures and with mouse, via context menu. Each type of interaction has it's own well defined purpose. While the user gestures are used to control the documents, the context menu takes care about configuring the application and opening documents.</div>
<div>Performing a right click on user screen, you will expand the following context menu.</div>
<table align='center' border='0'><tr><td><img src='http://i1114.photobucket.com/albums/k536/sandualbu/context_menu.png' /></td></tr></table>


<br /><br /><br />
<div><table border='0'><tr><td width='64px'><img src='http://i1114.photobucket.com/albums/k536/sandualbu/open.png' /></td><td><font color='#50308F' size='4'><b>1. Open</b></font></td></tr></table></div>
<div>This is the interface for opening documents. You can open a new document or from archive (recent).</div>

<br /><br />
<blockquote><div><table border='0'><tr><td><img src='http://i1114.photobucket.com/albums/k536/sandualbu/new.png' /></td><td><font color='#50308F' size='3'><b>1.1 Open a New Document</b></font></td></tr></table></div>
<div>Clicking on 'New' will open a dialog for selecting a file from your computer. The supported formats by KineSis are:</div></blockquote>

<ul><li>Microsoft Office Documents - <code>[</code>PowerPoint (pptx), Word (docx), Excel (xlsx)<code>]</code> - documents must be supported by Office 2007 or 2010.<br>
</li><li>Images - <code>[</code>jpg, jpeg, bmp, png, gif, tif, tiff<code>]</code>
</li><li>Text - <code>[</code>txt, source files<code>]</code> - common used ASCII files, as configured in 'Extensions.xml'</li></ul>

<div>After selecting a document, KineSis start processing it, starting with document pages. On the bottom of user screen, two loading bars are displayed: first with the progress of one page and the second with the progress of the whole document.</div>

<table align='center' border='0'><tr><td><img src='http://i1114.photobucket.com/albums/k536/sandualbu/pages_processing.png' /></td></tr></table>
<div>When processing of pages is done, the document is opened and the processing of charts starts in background. Because processing charts can take a while, the user is not suposed to wait until all document is processed, he can start presenting.</div>
<table align='center' border='0'><tr><td><img src='http://i1114.photobucket.com/albums/k536/sandualbu/charts_processing.png' /></td></tr></table>

<br /><br />
<blockquote><div><table border='0'><tr><td><img src='http://i1114.photobucket.com/albums/k536/sandualbu/existing.png' /></td><td><font color='#50308F' size='3'><b>1.2 Open an Existing Document</b></font></td></tr></table></div>
<div>KineSis saves all opened document in a directory specified in configurations. There are 2 reasons for this: all documents are converted in a common format (HTML), so images of pages and slides toghether with faces of 3D charts are stored in a well defined directory structure. More about this at Settings section.</div>
<div>Clicking on 'Existing' will open a form where you can select a document from archive and open or delete it from temporary directory.</div>
<table align='center' border='0'><tr><td><img src='http://i1114.photobucket.com/albums/k536/sandualbu/archive.jpg' /></td></tr></table></blockquote>

<br /><br /><br />
<div><table border='0'><tr><td width='64px'><img src='http://i1114.photobucket.com/albums/k536/sandualbu/settings.png' /></td><td><font color='#50308F' size='4'><b>2. Settings</b></font></td></tr></table></div>
<div>Clicking on 'Settings' will open a window where you can configure the application. KineSis works with Profiles, so more persons can use it from the same computer, each one with his own settings. A good example for this is a classroom where more teachers use the same computer for presentations.</div>v<br>
<table align='center' border='0'><tr><td><img src='http://i1114.photobucket.com/albums/k536/sandualbu/settings1.png' /></td></tr></table>
<div>You can select and apply an existing profile or you can create your own. The settings are related to screens, locations of profile temporary directory, slides and charts sizes, color theme and touch/untouch distances.</div>
<div><b>Profiles</b> contains a list of all KineSis profiles. Selecting a profile from the dropdown will update the entire settings form with selected profile's data.</div>
<div><b>Screens</b> contains 2 dropdowns containing all available system screens (monitors). You can define which screen will be used as user screen and which for presentation. When the system has more than one screen, the user and presentation screens cannot be the same. These are the defaults, the screens can be switched anytime.</div>
<div><b>Location</b> is the place where KineSis stores all temporary files for a profile. Changing this doesn't mean that all files will be moved to the new location.</div>
<div><b>Slides and Charts</b> contains settings for PowerPoint slides generator and chart generator.</div>

<ul><li><i>Slide Width</i> is the width of corresponding image of every generated chart. The height is adjusted automatically. The greater the value, the greater the quality of presentation with costs on performance.<br>
</li><li><i>Chart Width</i> have the same meaning as the Slide Width, but it reffers to image of a chart face size.<br>
</li><li><i>Chart Horizontal Faces</i> is the number of faces of a chart. For example, if this value is set to 4, you can think at a 3D chart as a cube, with 4 horizontal faces, each face containing a view of the chart from a different angle (90 degrees horizontally). If this is set to 0, then KineSis will not support 3D rotation for this profile.<br>
</li><li><i>Chart Vertical Faces</i> is the number of vertical views per horizontal view. Horizontal views contains images generated from Left-to-Right rotation and for each rotation, another set of views is assigned. These are the vertical views representing the Up-to-Down rotation or elevation.</li></ul>

<div>To have a better idea about how KineSis generates the views of 3D charts, think at the example with 4 horizontal faces. You have four views so far, meaning you can rotate the chart only from Left to Right or from Right to Left. Add 2 vertical faces for each horizontal face and you can rotate also 2 faces up and 2 faces down, this means you can also use the positive and negative elevation (not available for all type of charts, like 3D Pie). Sum these and you have 5 views per horizontal view. Having 4 horizontal views, you have a total of 20 views of a 3D chart, proportionnaly distributed in such a way that you can rotate the chart to view it from all important angles.</div>

<div><b>Theme</b> contains colors used by application. They can be changed by clicking on the right buttons labeled with HEX code of the color, which opens a color picker.</div>

<ul><li><i>Primary</i> is the primary color of user interface. This is used to color the main menu (center circle), the main (document) loading bar and the skeleton when a hand is raised up (indicating usage of secondary menu)<br>
</li><li><i>Secondary</i> is the color used in combination with primary color. This colors the submenus, user's joint points and the secondary (page) loading bar.<br>
</li><li><i>Background</i> is the application's background color.<br>
</li><li><i>Skeleton</i> is the skeleton color.</li></ul>


<div><b>Distances</b> contains settings related to touch and untouch distances.</div>

<ul><li><i>Touch Distance</i> represents the distance (in cm) from head to edge of the hand seen from above, the limit over which a hand is considered selected or active. Set this, for example at 40 and if you stretch a hand more than 40 centimeters, you can act on KineSis menus.<br>
</li><li><i>Untouch Distance</i> represents the limit under which a hand changes it's state, from active to inactive (selected to deselected).</li></ul>

<div><b>Save</b> contains options for saving a new profile. To activate the 'Save' button, you have to fill in a name (unique) for the profile you've just created. After saving, your profile name appears as selected in profile's dropdown.</div>
<div><b>Done</b> button applies the profile changes and closes the Settings window. If you made changes on an existing profile, you'll be asked to save the changes.</div>

<ul><li><i>Yes</i> will save the changes to profile and then the changes are applied to application. You cannot change the default (KineSis) profile.<br>
</li><li><i>No</i> will discard all changes and the window will be closed.<br>
</li><li><i>Cancel</i> will not save and will not close the window. You can make more changes or save a new profile.</li></ul>

<br /><br /><br />
<div><table border='0'><tr><td width='64px'><img src='http://i1114.photobucket.com/albums/k536/sandualbu/switch_screens.png' /></td><td><font color='#50308F' size='4'><b>3. Switch Screens</b></font></td></tr></table></div>
<div>This allow the user to change monitors (hardware) between user and presentation screen.</div>

<br /><br /><br />
<div><table border='0'><tr><td width='64px'><img src='http://i1114.photobucket.com/albums/k536/sandualbu/switch_minimal_view.png' /></td><td><font color='#50308F' size='4'><b>4. Switch Minimal View</b></font></td></tr></table></div>
<div>Switch between Normal and Minimal view. The Minimal View is designed to systems with only one screen. The functionality is very limited, user can only navigate through pages and scroll up or down.</div>
<table align='center' border='0'><tr><td><img src='http://i1114.photobucket.com/albums/k536/sandualbu/minimal.jpg' /></td></tr></table>

<br /><br /><br />
<div><table border='0'><tr><td width='64px'><img src='http://i1114.photobucket.com/albums/k536/sandualbu/exit.png' /></td><td><font color='#50308F' size='4'><b>5. Exit</b></font></td></tr></table></div>
<div>Leave the application.</div>
</font>