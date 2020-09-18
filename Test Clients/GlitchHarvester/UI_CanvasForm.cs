﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using RTCV.UI;

namespace RTCV.UI
{
    public partial class CanvasForm : Form
    {
        public static CanvasForm thisForm;
        public static List<CanvasForm> extraForms = new List<CanvasForm>();
        public ShadowPanel spForm;

        public static int spacerSize;
        public static int tileSize;

        static Dictionary<string, ComponentFormTile> loadedTileForms = new Dictionary<string, ComponentFormTile>();

        public bool SubFormMode
        {
            get {
                return (spForm != null);
            }
            set {
                if (value == false && spForm != null)
                    CloseSubForm();
                }
        }

        public CanvasForm(bool extraForm = false)
        {
            InitializeComponent();

            if (!extraForm)
            {
                thisForm = this;
                spacerSize = pnScale.Location.X;
                tileSize = pnScale.Size.Width;
                Controls.Remove(pnScale);
            }


        }

        public static ComponentFormTile getTileForm(string componentFormName, int? newSizeX = null, int? newSizeY = null, bool DisplayHeader = true)
        {

            if (!loadedTileForms.ContainsKey(componentFormName))
            {
                var newForm = (ComponentFormTile)Activator.CreateInstance(typeof(ComponentFormTile));
                loadedTileForms[componentFormName] = newForm;

                if (newSizeX != null && newSizeY != null)
                    newForm.SetCompoentForm(componentFormName, newSizeX.Value, newSizeY.Value, DisplayHeader);
            }

            return loadedTileForms[componentFormName];
        }

        public static int getTilePos(int gridPos)
        {
            return (gridPos * tileSize) + (gridPos * spacerSize) + spacerSize;
        }
        public static Point getTileLocation(int x, int y)
        {
            return new Point(getTilePos(x), getTilePos(y));
        }

        public static void unloadTileForms()
        {
            clearTileForms();
            loadedTileForms.Clear();
        }

        public static void clearTileForms()
        {
            thisForm.Controls.Clear();
            foreach (Form frm in extraForms)
            {
                frm.Controls.Clear();
                frm.Close();
            }
            extraForms.Clear();
            loadedTileForms.Clear();

        }

        public void ResizeCanvas(CanvasForm targetForm, CanvasGrid canvasGrid)
        {
            this.SetSize(getTilePos(canvasGrid.x), getTilePos(canvasGrid.y));
        }

        public void SetSize(int x, int y)
        {
            if (this.TopLevel)
                this.Size = new Size(x + CoreForm.xPadding, y + CoreForm.yPadding);
            else
                CoreForm.thisForm.SetSize(x, y);
        }

        public static void loadMultiGrid(MultiGrid mg)
        {
            mg.Load();
        }


        public static void loadTileForm(CanvasForm targetForm, CanvasGrid canvasGrid)
        {

            targetForm.ResizeCanvas(targetForm, canvasGrid);

            for (int x = 0; x < canvasGrid.x; x++)
                for (int y = 0; y < canvasGrid.y; y++)
                    if (canvasGrid.gridComponent[x, y] != null)
                    {
                        targetForm.Text = canvasGrid.GridName;
                        bool DisplayHeader = (canvasGrid.gridComponentDisplayHeader[x, y].HasValue ? canvasGrid.gridComponentDisplayHeader[x, y].Value : false);
                        var size = canvasGrid.gridComponentSize[x, y];
                        ComponentFormTile tileForm = getTileForm(canvasGrid.gridComponent[x, y], size?.Width, size?.Height, DisplayHeader);
                        tileForm.TopLevel = false;
                        targetForm.Controls.Add(tileForm);
                        tileForm.Location = getTileLocation(x, y);


                        tileForm.Show();
                    }



        }

        public static void loadTileFormExtraWindow(CanvasGrid canvasGrid, string WindowHeader = "RTC Extra Form")
        {
            CanvasForm extraForm = new CanvasForm(true);

            extraForm.Controls.Clear();
            extraForms.Add(extraForm);
            extraForm.FormBorderStyle = FormBorderStyle.FixedSingle;
            extraForm.MaximizeBox = false;
            extraForm.Text = WindowHeader;
            loadTileForm(extraForm, canvasGrid);
            extraForm.Show();


        }

        public static void loadTileFormMain(CanvasGrid canvasGrid)
        {
            clearTileForms();
            loadTileForm(thisForm, canvasGrid);
        }


        private void button4_Click(object sender, EventArgs e)
        {
            //test button, to delete later

            if (spForm == null)
            {
                ShowSubForm("ComponentFormSubForm");
            }
            else
                CloseSubForm();
        }

        public int getTileSpacesX()
        {
            int sizeX = this.Size.Width;
            int tilesSpace = sizeX - spacerSize;
            int nbSpaces = (int)((double)tilesSpace / (spacerSize + tileSize));

            return nbSpaces;
        }

        public int getTileSpacesY()
        {
            int sizeY = this.Size.Height;
            int tilesSpace = sizeY - spacerSize;
            int nbSpaces = (int)((double)tilesSpace / (spacerSize + tileSize));

            return nbSpaces;
        }


        public void ShowSubForm(string _type)
        {

            //sets program to SubForm mode, darkens screen and displays flating form.
            //Start by giving type of Form class. Implement interface SubForms.UI_SubForm for Cancel/Ok buttons

            //See DummySubForm for example

            if (spForm != null)
                CloseSubForm();

            spForm = new ShadowPanel(thisForm, _type);
            spForm.TopLevel = false;
            thisForm.Controls.Add(spForm);
            spForm.Show();
            spForm.BringToFront();
        }

        public void CloseSubForm()
        {
            //Closes subform and exists SubForm mode.
            //is automatically called when Cancel/Ok is pressed in SubForm.

            if (ShadowPanel.subForm != null) {
                ShadowPanel.subForm.Close();
                ShadowPanel.subForm = null;
            }

            if (spForm != null)
            {
                spForm.Close();
                spForm = null;
            }
        }

        private void CanvasForm_Load(object sender, EventArgs e)
        {

        }

    }
}
