using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Minesweeper.src
{
    public class DifficultyModes
    {
        public Difficulty BeginnerMode = new Difficulty("Beginner", false, 9, 9, 10);
        public Difficulty IntermediateMode = new Difficulty("Intermediate", false, 16, 16, 40);
        public Difficulty ExpertMode = new Difficulty("Expert", false, 30, 16, 99);
        public Difficulty CustomMode = new Difficulty("Custom", true, 30, 20, 145);

        public DifficultyModes()
        {

        }
    }

    public class Difficulty : INotifyPropertyChanged
    {
        private bool _isSelected;
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    OnPropertyChanged(nameof(IsSelected));

                    Debug.WriteLine(Title + "._isSelected: " + _isSelected);
                }
            }
        }

        public string Title { get; set; }

        private int _width;
        public int Width
        {
            get { return _width; }
            set
            {
                if (_width != value)
                {
                    if (value <= 9)
                        _width = 9;
                    else if (value > 9 && value <= 99)
                        _width = value;
                    else
                        _width = 99;

                    validateMines();

                    OnPropertyChanged(nameof(Width));
                    Debug.WriteLine("Width changed to " + Width + " !");
                }
            }
        }

        private int _height;
        public int Height
        {
            get { return _height; }
            set
            {
                if (_height != value)
                {
                    if (value <= 1)
                        _height = 1;
                    else if (value > 1 && value <= 99)
                        _height = value;
                    else
                        _height = 99;

                    validateMines();

                    OnPropertyChanged(nameof(Height));
                    Debug.WriteLine("Height changed to " + Height + " !");
                }
            }
        }

        private int _mines;
        public int Mines
        {
            get { return _mines; }
            set
            {
                if (_mines != value)
                {
                    if (value <= 0)
                        _mines = 0;
                    else if (value > 0 && value < Width * Height)
                        _mines = value;
                    else
                        _mines = (Width * Height) - 1;

                    OnPropertyChanged(nameof(Mines));
                    Debug.WriteLine("Mines changed to " + Mines + " !");
                }
            }
        }

        private void validateMines()
        {
            if (Mines > Width * Height)
                Mines = (Width * Height) - 1;
        }

        public bool IsCustom { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;

        public Difficulty(string Title, bool IsCustom, int Width, int Height, int Mines)
        {
            this.Title = Title;
            this.IsCustom = IsCustom;
            this.Width = Width;
            this.Height = Height;
            this.Mines = Mines;
        }
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


    }

    class DifficultyVM
    {
        public ObservableCollection<Difficulty> DifficultyList { get; set; }

        public DifficultyVM()
        {
            DifficultyModes difficultyModes = new DifficultyModes();

            DifficultyList = new ObservableCollection<Difficulty>();
            DifficultyList.Add(difficultyModes.BeginnerMode);
            DifficultyList.Add(difficultyModes.IntermediateMode);
            DifficultyList.Add(difficultyModes.ExpertMode);
            DifficultyList.Add(difficultyModes.CustomMode);
        }
    }
}
