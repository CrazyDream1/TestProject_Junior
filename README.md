## Баги:

1) В карточке пациента можно добавить дату рождения в будущем.
Такая же проблема в форме обращении.

Добавим проверку даты в функции AddModifyPatient - btnSubmit_Click и AddModifyRequest - btnSubmit_Click
```
if (dateOfBirth > DateTime.Now)
{
    status = false;
    messageBuilder.Append("Дата рождения - не может быть в будущем.\n");
}
```

2) При вводе даты рождения 00.00.0001 приложение падает.
Такая же проблема в обращении.

1 Января 1753 года - минимальное значение для даты в MS SQL Server.
```
class BaseDate
{
    public static readonly DateTime BaseDateConst = new DateTime(1753, 1, 1);
}
```
Поэтому добавим проверку, что введенная дата не меньше этой даты
в функциях AddModifyPatient - btnSubmit_Click и AddModifyRequest - btnSubmit_Click.

```
if (dateOfBirth < BaseDate.BaseDateConst)
{
    status = false;
    messageBuilder.Append("Дата рождения - не может быть настолько в прошлом.\n");
}
```

3) При редактировании существующего обращения пациента, приложение падает

При попытке изменения записи создавался новый пользователь, а не изменялся выбранный (класс AddModifyRequest, строчка 118). Я изменил это.

```
m_request.Patient = new PatientCard(); 
=>
m_request.Patient = m_patient;
```