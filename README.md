# hhru-cvupdater
Программа для автообновления  информации соискателя на сайте hh.ru

Версия программы реализованная конслольным приложением для настольных windows систем.
Одиночный запуск приложения обновляет дату публикации всех доступных резюме.

## Сборка и настройка

Для сборки необходимо иметь Visual Studio 2012 и выше.

Для настройки необходимо изменить параметр   wr.Headers[HttpRequestHeader.Authorization] в файле Program.cs
в формате  wr.Headers[HttpRequestHeader.Authorization] = "Bearer {access_token}"

{access_token} можно получить по адресу https://dev.hh.ru/admin
Нажатием на кнопку добавить токен
Стоит отметить, что {access_token} имеет срок жизни и его необходимо периодически обновлять.

## Способ применения 

После настройки и сборки проекта, можно автоматизировать обновление путем добавления задания в стандартный планирощик заданий

Инструкция:
1. Открыть планировщик заданий 
2. Создать задачу
3. На вкладке общие указать имя
4. На вкладке триггеры нажать создать
  4.1. Выбрать условия для расписания запуска и подтвердить.
5. На вкладке действие нажать создать
  5.1 Выбрать действие: запуск программы
  5.2 Указать путь к exe файлу программы и подтвердить
6. Нажать Ок.
7. Убедиться, что в библиотеке планировщика появилось созданное задание
  
