﻿using ForgeModBuilder.Gradle;
using ForgeModBuilder.Managers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace ForgeModBuilder.Forms
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            InitialiseEventHandlers();
            SizeChanged += ResizeForm;
            Load += (sender, e) =>
            {
                ResizeForm(null, null);
            };
        }

        private void ResizeForm(object sender, EventArgs e)
        {
            CommandEntryTextBox.Width = ConsoleBottomPanel.Width - ExecuteCommandButton.Width - 4;
            ExecuteCommandButton.Location = new Point(CommandEntryTextBox.Width + 4, ExecuteCommandButton.Location.Y);
            ForgeModBuilder.MainFormInstance.ProjectsListView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
        }

        private void InitialiseEventHandlers()
        {
            LastConsoleMessageLabel.TextChanged += LastConsoleMessageLabelTextChanged;
            ExecuteCommandButton.Click += ExecuteCommandButtonClick;
            ConsoleTextBox.LinkClicked += ConsoleTextBoxLinkClicked;
            OpenProjectButton.Click += OpenProjectClick;
            ProjectsListView.SelectedIndexChanged += SelectProject;
            openToolStripMenuItem.Click += OpenProjectClick;
            openToolStripMenuItem1.Click += OpenProjectClick;
            renameToolStripMenuItem.Click += RenameProjectsClick;
            renameToolStripMenuItem1.Click += RenameProjectsClick;
            removeToolStripMenuItem.Click += RemoveProjectsClick;
            removeToolStripMenuItem1.Click += RemoveProjectsClick;
            groupToolStripMenuItem.Click += NewGroupClick;
            groupToolStripMenuItem1.Click += NewGroupClick;
            newGroupToolStripMenuItem.Click += NewGroupClick;
            newGroupToolStripMenuItem1.Click += NewGroupClick;
        }

        private void LastConsoleMessageLabelTextChanged(object sender, EventArgs e)
        {
            Image fakeImage = new Bitmap(1, 1);
            Graphics graphics = Graphics.FromImage(fakeImage);
            SizeF size = graphics.MeasureString(LastConsoleMessageLabel.Text, LastConsoleMessageLabel.Font);
            LastConsoleMessageLabel.Location = new Point(ClientRectangle.Width - (int)size.Width - 5, LastConsoleMessageLabel.Location.Y);
        }

        private void ExecuteCommandButtonClick(object sender, EventArgs e)
        {
            GradleExecuter.RunGradleCommand(CommandEntryTextBox.Text);
            CommandEntryTextBox.Text = "";
        }

        private void ConsoleTextBoxLinkClicked(object sender, LinkClickedEventArgs e)
        {
            Process.Start(e.LinkText);
        }

        private void OpenProjectClick(object sender, EventArgs e)
        {
            ProjectManager.OpenProject();
        }

        private void SelectProject(object sender, EventArgs e)
        {
            if (ProjectsListView.SelectedItems.Count == 1)
            {
                // Add check?
                ProjectManager.CurrentProject = (Project)ProjectsListView.SelectedItems[0].Tag;
                renameToolStripMenuItem.Enabled = true;
                renameToolStripMenuItem1.Enabled = true;
                removeToolStripMenuItem.Enabled = true;
                removeToolStripMenuItem1.Enabled = true;
                groupToolStripMenuItem.Enabled = true;
                groupToolStripMenuItem1.Enabled = true;
            }
            else if (ProjectsListView.SelectedItems.Count == 0)
            {
                ProjectManager.CurrentProject = null;
                renameToolStripMenuItem.Enabled = false;
                renameToolStripMenuItem1.Enabled = false;
                removeToolStripMenuItem.Enabled = false;
                removeToolStripMenuItem1.Enabled = false;
                groupToolStripMenuItem.Enabled = false;
                groupToolStripMenuItem1.Enabled = false;
            }
        }

        private void RenameProjectsClick(object sender, EventArgs e)
        {
            if (ProjectsListView.SelectedItems.Count > 0)
            {
                ProjectsListView.SelectedItems[0].BeginEdit();
            }
        }

        private void RemoveProjectsClick(object sender, EventArgs e)
        {
            if (ProjectsListView.SelectedItems.Count > 0)
            {
                int count = ProjectsListView.SelectedItems.Count;
                for (int i = 0; i < count; i++)
                {
                    ProjectManager.Projects.Remove((Project)ProjectsListView.SelectedItems[0].Tag);
                    ProjectsListView.Items.Remove(ProjectsListView.SelectedItems[0]);
                    // Check?
                }
            }
        }

        private void NewGroupClick(object sender, EventArgs e)
        {
            if (ProjectsListView.SelectedItems.Count > 0)
            {
                AddGroupForm form = new AddGroupForm();
                if (form.ShowDialog() == DialogResult.Cancel)
                {
                    return;
                }
                ListViewGroup group = new ListViewGroup(form.GroupNameTextBox.Text);
                ProjectsListView.Groups.Add(group);
                foreach (ListViewItem item in ProjectsListView.SelectedItems)
                {
                    item.Group = group;
                }
                ToolStripMenuItem menuItem = new ToolStripMenuItem();
                menuItem.Text = group.Header;
                menuItem.Click += (sender1, e1) =>
                {
                    if (ProjectsListView.SelectedItems.Count > 0)
                    {
                        foreach (ListViewItem item in ProjectsListView.SelectedItems)
                        {
                            item.Group = group;
                        }
                    }
                };
                groupToolStripMenuItem.DropDownItems.Insert(0, menuItem);
                groupToolStripMenuItem1.DropDownItems.Insert(0, menuItem);

                menuItem = new ToolStripMenuItem();
                menuItem.Text = "No group";
                menuItem.Click += (sender1, e1) =>
                {
                    if (ProjectsListView.SelectedItems.Count > 0)
                    {
                        foreach (ListViewItem item in ProjectsListView.SelectedItems)
                        {
                            item.Group = null;
                        }
                    }
                };

                if (groupToolStripMenuItem.DropDownItems.Count == 2)
                {
                    groupToolStripMenuItem.DropDownItems.Insert(1, new ToolStripSeparator());
                    groupToolStripMenuItem.DropDownItems.Insert(2, menuItem);
                }
                if (groupToolStripMenuItem1.DropDownItems.Count == 2)
                {
                    groupToolStripMenuItem1.DropDownItems.Insert(1, new ToolStripSeparator());
                    groupToolStripMenuItem1.DropDownItems.Insert(2, menuItem);
                }
            }
        }
    }
}
