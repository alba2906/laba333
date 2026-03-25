using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using Microsoft.Win32;
using Laba1.Core;

namespace Laba1.Windows
{
    public partial class Compiler : Window
    {
        private readonly LexicalAnalyzer _lexicalAnalyzer = new();
        private readonly SyntaxAnalyzer _syntaxAnalyzer = new();
        private string _currentFilePath = string.Empty;

        public Compiler()
        {
            InitializeComponent();
        }

        // Кнопка "Создать"
        private void Create_Click(object sender, RoutedEventArgs e)
        {
            FileContentViewer.Document = new FlowDocument();
            _currentFilePath = string.Empty;
        }

        // Кнопка "Открыть"
        private void Open_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Текстовые файлы (*.txt)|*.txt|Все файлы (*.*)|*.*",
                Title = "Открыть файл"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    string content = File.ReadAllText(openFileDialog.FileName);
                    FileContentViewer.Document = new FlowDocument(
                        new Paragraph(new Run(content)));
                    _currentFilePath = openFileDialog.FileName;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        $"Ошибка при открытии файла: {ex.Message}",
                        "Ошибка",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
            }
        }

        // Кнопка "Сохранить"
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(_currentFilePath))
            {
                SaveAs_Click(sender, e);
                return;
            }

            try
            {
                TextRange textRange = new(
                    FileContentViewer.Document.ContentStart,
                    FileContentViewer.Document.ContentEnd);

                File.WriteAllText(_currentFilePath, textRange.Text);

                MessageBox.Show(
                    "Файл успешно сохранён.",
                    "Сохранение",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Ошибка при сохранении файла: {ex.Message}",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        // Кнопка "Сохранить как"
        private void SaveAs_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "Текстовые файлы (*.txt)|*.txt|Все файлы (*.*)|*.*",
                Title = "Сохранить файл"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    TextRange textRange = new(
                        FileContentViewer.Document.ContentStart,
                        FileContentViewer.Document.ContentEnd);

                    File.WriteAllText(saveFileDialog.FileName, textRange.Text);
                    _currentFilePath = saveFileDialog.FileName;

                    MessageBox.Show(
                        "Файл успешно сохранён.",
                        "Сохранение",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        $"Ошибка при сохранении файла: {ex.Message}",
                        "Ошибка",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
            }
        }

        // Кнопка "Выход"
        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        // Событие изменения текста в RichTextBox
        private void FileContentViewer_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Здесь можно добавить логику при изменении текста
            // Например, подсветку синтаксиса или валидацию
        }

        // Кнопка "Вызов справки"
        private void Reference_Click(object sender, RoutedEventArgs e)
        {
            // Открыть окно справки или показать информацию
            MessageBox.Show(
                "Справка по программе:\n\n" +
                "1. Введите или откройте исходный код.\n" +
                "2. Нажмите кнопку 'Пуск' для анализа.\n" +
                "3. Результаты отобразятся в таблице.\n" +
                "4. Кликните по ошибке для перехода к ней в тексте.",
                "Справка",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        // Кнопка "О программе"
        private void About_Click(object sender, RoutedEventArgs e)
        {
            AboutWindow aboutWindow = new AboutWindow
            {
                Owner = this
            };
            aboutWindow.ShowDialog();
        }

        // Кнопка "Пуск" - запуск лексического и синтаксического анализа
        private void RunAnalysis_Click(object sender, RoutedEventArgs e)
        {
            TextRange textRange = new(
                FileContentViewer.Document.ContentStart,
                FileContentViewer.Document.ContentEnd);

            string sourceText = textRange.Text;

            OutputDataGrid.ItemsSource = null;

            // Лексический анализ
            AnalysisResult lexicalResult = _lexicalAnalyzer.Analyze(sourceText);
            List<ResultRow> rows = new List<ResultRow>();

            if (lexicalResult.HasErrors)
            {
                foreach (LexicalError error in lexicalResult.Errors)
                {
                    rows.Add(new ResultRow
                    {
                        Code = "99",
                        TypeName = "лексическая ошибка",
                        Lexeme = error.LexemeRepresentation,
                        Location = error.Location,
                        Description = error.Message,
                        Line = error.Line,
                        Column = error.Column,
                        IsError = true
                    });
                }

                OutputDataGrid.ItemsSource = rows;

                MessageBox.Show(
                    $"Обнаружены лексические ошибки: {lexicalResult.Errors.Count}",
                    "Результат анализа",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            // Синтаксический анализ
            SyntaxAnalysisResult syntaxResult = _syntaxAnalyzer.Analyze(lexicalResult.Tokens);

            if (!syntaxResult.HasErrors)
            {
                OutputDataGrid.ItemsSource = new List<ResultRow>();

                MessageBox.Show(
                    "Синтаксический анализ завершён успешно. Ошибок не обнаружено.",
                    "Результат анализа",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                return;
            }

            foreach (SyntaxError error in syntaxResult.Errors)
            {
                rows.Add(new ResultRow
                {
                    Code = "SYN",
                    TypeName = "синтаксическая ошибка",
                    Lexeme = error.InvalidFragment,
                    Location = error.Location,
                    Description = error.Message,
                    Line = error.Line,
                    Column = error.Column,
                    IsError = true
                });
            }

            OutputDataGrid.ItemsSource = rows;

            MessageBox.Show(
                $"Синтаксический анализ завершён. Найдено ошибок: {syntaxResult.Errors.Count}",
                "Результат анализа",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);
        }

        // Переход к ошибке при клике на строке таблицы
        private void OutputDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (OutputDataGrid.SelectedItem is not ResultRow row || !row.IsError)
                return;

            TextPointer? pointer = TextPositionHelper.GetTextPointerAt(
                FileContentViewer,
                row.Line,
                row.Column);

            if (pointer == null)
                return;

            FileContentViewer.Focus();
            FileContentViewer.CaretPosition = pointer;
            FileContentViewer.Selection.Select(pointer, pointer);
        }
    }
}