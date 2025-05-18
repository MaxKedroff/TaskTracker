namespace TaskTracker.Models.DTO
{
    public class GanttItemDTO
    {
        public int Id { get; set; }

        /// <summary>Название</summary>
        public string Title { get; set; }

        /// <summary>Дата начала (для эпика можно взять DateCreated или спец-поле)</summary>
        public DateTime Start { get; set; }

        /// <summary>Дата окончания (DeadLine)</summary>
        public DateTime End { get; set; }

        /// <summary>
        /// Для эпиков — null;  
        /// для простых задач — TaskId того эпика, к которому они привязаны
        /// </summary>
        public int? ParentId { get; set; }

        /// <summary>Маркер “эпик/не эпик” (можно не возвращать, если фронт берёт по ParentId)</summary>
        public bool IsEpic { get; set; }

    }
}
