using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevitHP
{
    public class Model : INotifyPropertyChanged
    {

        public Model() { }
        public Model(int id, string mod_name, string mod_size, int catalogid, string md5, String audit,string datatime)
        {
            Id = id;
            Mod_Name = mod_name;
            Mod_Size = mod_size;
            CatalogId = catalogid;
            MD5 = md5;     
            Audit = audit;
            DataTime = datatime;

        }

        private bool _isSelected = false;

        public event PropertyChangedEventHandler PropertyChanged;

        public bool IsSelected
        {
            get { return _isSelected; }

            set
            {
                _isSelected = value;
                OnPropertyChanged("IsSelected");
            }
        }
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        public int Id { get; set; }
        public int id
        {
            get { return id; }
            set
            {
                if (this.Id != value)
                {
                    this.id = value;
                    OnPropertyChanged("Id");
                }
            }
        }

        public int NameID { get; set; }
        public int nameid
        {
            get { return nameid; }
            set
            {
                if (this.NameID != value)
                {
                    this.nameid = value;
                    OnPropertyChanged("NameID");
                }
            }
        }
        public String Audit { get; set; }
        public String audit
        {
            get { return audit; }
            set
            {
                if (this.Audit != value)
                {
                    this.audit = value;
                    OnPropertyChanged("Audit");
                }
            }
        }

       

        public string DataTime { get; set; }
        public string datatime
        {
            get { return datatime; }
            set
            {
                if (this.DataTime != value)
                {
                    this.datatime = value;
                    OnPropertyChanged("DataTime");
                }
            }

        }

        public string Mod_Name { get; set; }
        public string mod_name
        {
            get { return mod_name; }
            set
            {
                if (this.Mod_Name != value)
                {
                    this.mod_name = value;
                    OnPropertyChanged("Mod_Name");
                }
            }
        }

        public string Mod_Size { get; set; }
        public string mod_size
        {
            get { return Mod_Size; }
            set
            {
                if (this.Mod_Size != value)
                {
                    this.mod_size = value;
                    OnPropertyChanged("Mod_Size");
                }
            }
        }

        public int CatalogId { get; set; }
        public int catalogid
        {
            get { return CatalogId; }
            set
            {
                if (this.CatalogId != value)
                {
                    this.catalogid = value;
                    OnPropertyChanged("CatalogId");
                }
            }
        }

        public string md5 { get; set; }

        public string MD5
        {
            get { return md5; }
            set
            {
                if (this.MD5 != value)
                {
                    this.md5 = value;
                    OnPropertyChanged("MD5");
                }
            }
        }

    }
}
