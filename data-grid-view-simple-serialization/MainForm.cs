using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Newtonsoft.Json;

namespace data_grid_view_simple_serialization
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            // Create a file path to persist the data.
            var appDataFolder = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                GetType().Assembly.GetName().Name
            ); ;
            Directory.CreateDirectory(appDataFolder);
            _jsonPath = Path.Combine(appDataFolder, "notes.json");
        }
        readonly string _jsonPath;
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            dataGridView.DataSource = Notes;

            // Generate columns
            Notes.Add(new Note());
            dataGridView.Columns[nameof(Note.Title)].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dataGridView.Columns[nameof(Note.Message)].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            Notes.Clear();

            // Read in the saved notes from last time.
            if (File.Exists(_jsonPath))
            {
                var json = File.ReadAllText(_jsonPath);
                var saved = JsonConvert.DeserializeObject<Note[]>(json);
                foreach (var note in saved)
                {
                    Notes.Add(note);
                }
            }

            // Actions that trigger a save
            dataGridView.CellEndEdit += (sender, e) => save();
            dataGridView.RowsRemoved += (sender, e) => save();
        }

        private void save()
        {
            // Do not block on this method
            BeginInvoke((MethodInvoker)delegate 
            {
                var onlyValidNotes = Notes.Where(_ => 
                 !string.IsNullOrWhiteSpace(_.Title) ||
                 !string.IsNullOrWhiteSpace(_.Message));
                File.WriteAllText(
                    _jsonPath, 
                    JsonConvert.SerializeObject(onlyValidNotes, Formatting.Indented));
            });
        }
        BindingList<Note> Notes = new BindingList<Note>();
    }
    class Note
    {
        public string Title { get; set; }
        public string Message { get; set; }
    }
}
