using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Linq;

namespace BuildTreeOfImage
{
    public partial class Form1 : Form
    {
        string[] _fileNamesList;
        int _numFile = 0;
        string rootFolderDirectory = null;

        public Form1()
        {
            InitializeComponent();
            //Открытие окна выбора корневого каталога
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            MessageBox.Show("Выберете корневой каталог, если в каталоге нет изображений, выберете другой", "Примечание к программе");
            //В случае если в выбранной директории нет изображений, выбрать еще раз директорию
            while (treeView1.Nodes.Count == 0)
            {
                DialogResult result = fbd.ShowDialog();
                rootFolderDirectory = fbd.SelectedPath;
                //Закрыть приложение при нажатии кнопок Отмена и Выход
                if (result == DialogResult.Cancel)
                {
                    Close();
                    break;
                }
                //При нажатии кнопки ОК запускаем процесс построения дерева
                if (result == DialogResult.OK)
                {
                    //Вызов метода для работы с каталогами
                    DirectoryInfo directoryInfo = new DirectoryInfo(rootFolderDirectory);
                    //Вызов метода построения дерева
                    BuildTree(directoryInfo, treeView1.Nodes);
                }
            }
        }

        //Процесс получения папок и файлов
        private void BuildTree(DirectoryInfo directoryInfo, TreeNodeCollection addInMe)
        {
            //Добавляем новый узел в коллекцию Nodes с именем и указанием ключа "Folder"
            TreeNode selectedNode = addInMe.Add("Folder", directoryInfo.Name);

            //Перебираем папки.
            foreach (DirectoryInfo subdir in directoryInfo.GetDirectories())
            {
                //Запускам процесс получения папок с текущей найденной директории
                //Когда все папки пройдут, пойдет поиск файлов в папках обратно по рекурсии
                BuildTree(subdir, selectedNode.Nodes);
            }

            //Перебираем файлы
            foreach (FileInfo file in directoryInfo.GetFiles())
            {
                //Поиск изображений в директории, в случае нахождения 
                //добавляем новый узел в коллекцию Nodes с именем и указанием ключа "File"
                if (file.Name.Contains(".jpg")
                    || file.Name.Contains(".jpeg")
                    || file.Name.Contains(".bmp")
                    || file.Name.Contains(".PNG")
                    || file.Name.Contains(".JPG")
                    || file.Name.Contains(".JPEG")
                    || file.Name.Contains(".gif")
                    || file.Name.Contains(".png"))
                    selectedNode.Nodes.Add("File", file.Name);
            }
            //Удаляем пустые узлы подкаталогов
            if (selectedNode.Nodes.Count == 0)
                selectedNode.Remove();
        }

        // Выбор изображения в дереве 
        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            try
            {
                //***Для слайд-шоу
                //Путь от диска до выбранной директории включая ее
                var str1 = e.Node.FullPath.Split(new char[] { '\\' });
                var l1 = str1.Length - 1;
                var path1 = rootFolderDirectory.Replace(str1[l1], "");
                //Путь от диска до изображения
                var path2 = path1 + e.Node.FullPath.Replace(str1[0], "");
                //Путь от диска до каталога с выбранным изображением
                var str2 = path2.Split(new char[] { '\\' });
                var l2 = str2.Length - 1;
                if (str2[l2] != "")
                {
                    var path3 = path2.Replace(str2[l2], "");
                    //фильтр изображений для слайд шоу
                    var ext1 = Directory.GetFiles(path3, "*.jp*g");
                    var ext2 = Directory.GetFiles(path3, "*.png");
                    var ext3 = Directory.GetFiles(path3, "*.bmp");
                    var ext4 = Directory.GetFiles(path3, "*.gif");
                    var ext5 = Directory.GetFiles(path3, "*.PNG");
                    var ext6 = Directory.GetFiles(path3, "*.JP*G");
                    //Список изображений для слайд-шоу
                    _fileNamesList = ext1.Concat(ext2).Concat(ext3).Concat(ext4).Concat(ext5).Concat(ext6).ToArray();

                    if (!checkBox1.Checked)
                    {
                        try
                        {
                            timer1.Enabled = false;
                            //Отрисовываем выбранное изображение в pictureBox'е
                            pictureBox1.Image = Image.FromFile(FindPath() + e.Node.FullPath);
                        }
                        catch { }
                    }
                }
            }
            catch { }
        }

        private string FindPath()
        {
            //***Для вывода изображения
            //Извлечение пути файла
            //Путь от диска до выбранной директории
            var str = rootFolderDirectory.Split(new char[] { '\\' });
            var l = str.Length - 1;
            var path = rootFolderDirectory.Replace(str[l], "");
            return path;
        }
        
        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                if (_fileNamesList.Length > 0)
                {
                    if (checkBox1.Checked)
                    {
                        try
                        {
                            //Назначаем новую картинку
                            pictureBox1.Image = Image.FromFile(_fileNamesList[_numFile]);
                            //Наращиваем счётчик
                            _numFile++;
                            if (_numFile > _fileNamesList.Length - 1)
                                _numFile = 0;
                        }
                        catch
                        {
                            //Обнуляем счетчик и выводим изображение
                            _numFile = 0;
                            pictureBox1.Image = Image.FromFile(_fileNamesList[_numFile]);
                        }
                    }
                }
            }
            catch { }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            timer1.Enabled = true;
        }
    }
}
