using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using Kbg.NppPluginNET.PluginInfrastructure;

namespace Kbg.NppPluginNET
{
    class Main
    {
        internal const string PluginName = "AllOccurenceSelector";
        static string iniFilePath = null;
        static bool someSetting = false;
        static frmMyDlg frmMyDlg = null;
        static int idMyDlg = -1;

        // toolbar icons
        //static Bitmap tbBmp_color = Properties.Resources.star;     // standard icon small color
        //static Icon tbIco_black = Properties.Resources.star_black; // Fluent UI icon black
        //static Icon tbIco_white = Properties.Resources.star_white; // Fluent UI icon white
        //static Icon tbIcon = null;

        public static void OnNotification(ScNotification notification)
        {  
            // This method is invoked whenever something is happening in notepad++
            // use eg. as
            // if (notification.Header.Code == (uint)NppMsg.NPPN_xxx)
            // { ... }
            // or
            //
            // if (notification.Header.Code == (uint)SciMsg.SCNxxx)
            // { ... }
        }

        internal static void CommandMenuInit()
        {
            StringBuilder sbIniFilePath = new StringBuilder(Win32.MAX_PATH);
            Win32.SendMessage(PluginBase.nppData._nppHandle, (uint) NppMsg.NPPM_GETPLUGINSCONFIGDIR, Win32.MAX_PATH, sbIniFilePath);
            iniFilePath = sbIniFilePath.ToString();
            if (!Directory.Exists(iniFilePath)) Directory.CreateDirectory(iniFilePath);
            iniFilePath = Path.Combine(iniFilePath, PluginName + ".ini");
            someSetting = (Win32.GetPrivateProfileInt("SomeSection", "SomeKey", 0, iniFilePath) != 0);

            PluginBase.SetCommand(0, "Whole Words Selector", SelectWholeWordOccurences, new ShortcutKey(false, false, true, Keys.D3));
            PluginBase.SetCommand(1, "All Chars Selector", SelectAllCharOccurences, new ShortcutKey(false, false, true, Keys.D4)); idMyDlg = 1;
        }

        internal static void SetToolBarIcon()
        {
            // create struct
            toolbarIcons tbIcons = new toolbarIcons();

            // add bmp icon
            //tbIcons.hToolbarBmp = tbBmp_color.GetHbitmap();
            //tbIcons.hToolbarIcon = tbIco_black.Handle;            // icon with black lines
            //tbIcons.hToolbarIconDarkMode = tbIco_white.Handle;  // icon with light grey lines

            // convert to c++ pointer
            IntPtr pTbIcons = Marshal.AllocHGlobal(Marshal.SizeOf(tbIcons));
            Marshal.StructureToPtr(tbIcons, pTbIcons, false);

            // call Notepad++ api
            Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_ADDTOOLBARICON_FORDARKMODE, PluginBase._funcItems.Items[idMyDlg]._cmdID, pTbIcons);

            // release pointer
            Marshal.FreeHGlobal(pTbIcons);
        }
		
        internal static void PluginCleanUp()
        {
            Win32.WritePrivateProfileString("SomeSection", "SomeKey", someSetting ? "1" : "0", iniFilePath);
        }

        internal static void SelectAllCharOccurences()
        {
            //create a scintilla gateway and provide it with the pointer to the current scintilla
            IntPtr currentScint = PluginBase.GetCurrentScintilla();
            ScintillaGateway scintillaGateway = new ScintillaGateway(currentScint);

            try
            {
                int length = scintillaGateway.GetLength(); //get length of string in current window
                string highlightedString = scintillaGateway.GetSelText(); //get the selected string

                List<int> startIndices = new List<int>(); //create array of start indices of all occurences
                List<int> endIndices = new List<int>(); //create array of end indices of all occurences
                int searchStartIndex = 0; //start searching from the beginning of the document

                while (true) //loop to find start and end indices of all occurences
                {
                    TextToFind textToFind = new TextToFind(new PluginInfrastructure.CharacterRange(searchStartIndex, length), highlightedString); //typecast highlighted string to type TextToFind
                    int currentStartIndex = scintillaGateway.FindText(FindOption.MATCHCASE, textToFind); //get start index of first occurence of highlighted string

                    if (currentStartIndex == -1)
                    {
                        break; //exit the loop if no more occurences
                    }

                    int currentEndIndex = currentStartIndex + highlightedString.Length; //set end index of current string
                    startIndices.Add(currentStartIndex); //add this occurence's start index to array of start indices
                    endIndices.Add(currentEndIndex); //add this ocurrence's end index to array of end indices
                    searchStartIndex = currentEndIndex; //set the next search starting position to end of current occurence

                    if (searchStartIndex >= length)
                    {
                        break; //exit the loop if the search reaches the end of document
                    }
                }

                for (int i = 0; i < startIndices.Count; i++) //loop to select all occurences
                {
                    int startIndex = startIndices[i]; //get start index of this occurence
                    int endIndex = endIndices[i]; //get end index of this occurence
                    if (i == 0)
                    {
                        scintillaGateway.SetSelection(startIndex, endIndex); //remove extra cursor at end of original selection
                        continue;
                    }
                    scintillaGateway.AddSelection(startIndex, endIndex); //highlight this occurence
                    scintillaGateway.SetAdditionalSelectionTyping(true);
                }
                scintillaGateway.SetAdditionalCaretsBlink(true);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        internal static void SelectWholeWordOccurences()
        {
            //create a scintilla gateway and provide it with the pointer to the current scintilla
            IntPtr currentScint = PluginBase.GetCurrentScintilla();
            ScintillaGateway scintillaGateway = new ScintillaGateway(currentScint);

            try
            {
                int length = scintillaGateway.GetLength(); //get length of string in current window
                string highlightedString = scintillaGateway.GetSelText(); //get the selected string

                List<int> startIndices = new List<int>(); //create array of start indices of all occurences
                List<int> endIndices = new List<int>(); //create array of end indices of all occurences
                int searchStartIndex = 0; //start searching from the beginning of the document

                while (true) //loop to find start and end indices of all occurences
                {
                    TextToFind textToFind = new TextToFind(new PluginInfrastructure.CharacterRange(searchStartIndex, length), highlightedString); //typecast highlighted string to type TextToFind
                    int currentStartIndex = scintillaGateway.FindText(FindOption.WHOLEWORD, textToFind); //get start index of first occurence of highlighted string

                    if (currentStartIndex == -1)
                    {
                        break; //exit the loop if no more occurences
                    }

                    int currentEndIndex = currentStartIndex + highlightedString.Length; //set end index of current string
                    startIndices.Add(currentStartIndex); //add this occurence's start index to array of start indices
                    endIndices.Add(currentEndIndex); //add this ocurrence's end index to array of end indices
                    searchStartIndex = currentEndIndex; //set the next search starting position to end of current occurence

                    if (searchStartIndex >= length)
                    {
                        break; //exit the loop if the search reaches the end of document
                    }
                }

                for (int i = 0; i < startIndices.Count; i++) //loop to select all occurences
                {
                    int startIndex = startIndices[i]; //get start index of this occurence
                    int endIndex = endIndices[i]; //get end index of this occurence
                    if (i == 0)
                    {
                        scintillaGateway.SetSelection(startIndex, endIndex); //remove extra cursor at end of original selection
                        continue;
                    }
                    scintillaGateway.AddSelection(startIndex, endIndex); //highlight this occurence
                    scintillaGateway.SetAdditionalSelectionTyping(true);
                }
                scintillaGateway.SetAdditionalCaretsBlink(true);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}