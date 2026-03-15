namespace IRT.Modules.DataTransfer.Generic.Domain.ValueObjects.Veeva
{
    // TODO - Move with GenericWebApiNotificationSenderService in EDC
    public enum RequestType
    {
        CreateCasebook = 0,
        SetSubjectStatus = 1,
        CreateEventGroups = 2,
        SetEventDate = 3,
        SetItemValue = 4,
        EditSubmittedForm = 5,
        SubmitForm = 6,
        Authorization = 7,
        UnsetSubjectStatus = 8,
        CreateForms = 9,
        UpsertForms = 10,
        CombinationFormDataUpdate = 11,
        SetEventAsDidNotOccur = 12,
        UpsertEventGroups = 13
    }
}