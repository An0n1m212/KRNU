using TestingSystem.Abstractions;
using TestingSystem.Models;

namespace TestingSystem.Services;

/// <summary>Правила доступу до лекцій; працює поверх ILectureRepository та IAuthService.</summary>
public sealed class LectureService : ILectureService
{
    private readonly ILectureRepository _lectures;
    private readonly IAuthService _auth;

    public LectureService(ILectureRepository lectures, IAuthService auth)
    {
        _lectures = lectures;
        _auth = auth;
    }

    public IReadOnlyList<Lecture> GetManageable()
        => _lectures.GetAll().Where(_auth.CanManage).OrderByDescending(l => l.UpdatedAt).ToList();

    public IReadOnlyList<Lecture> GetForCurrentStudent()
    {
        var user = _auth.GetCurrentUser();
        if (user is null || user.Role != Role.Student) return Array.Empty<Lecture>();

        return _lectures.GetAll()
            .Where(l => l.IsPublished && (l.AssignedGroupIds.Count == 0 || (user.GroupId is int g && l.AssignedGroupIds.Contains(g))))
            .OrderByDescending(l => l.UpdatedAt).ToList();
    }

    public Lecture? GetById(int id)
    {
        var lecture = _lectures.GetById(id);
        if (lecture is null) return null;

        var user = _auth.GetCurrentUser();
        bool visible = _auth.CanManage(lecture)
            || (user is { Role: Role.Student } && lecture.IsPublished
                && (lecture.AssignedGroupIds.Count == 0 || (user.GroupId is int g && lecture.AssignedGroupIds.Contains(g))));
        return visible ? lecture : null;
    }

    public Lecture Save(Lecture lecture)
    {
        var user = _auth.GetCurrentUser();
        if (user is null || !_auth.IsInRole(Role.Teacher, Role.Admin)) throw new UnauthorizedAccessException();

        var now = DateTime.Now;
        if (lecture.Id == 0)
        {
            lecture.AuthorTeacherId = user.Id;
            lecture.CreatedAt = now;
        }
        else
        {
            var existing = _lectures.GetById(lecture.Id) ?? throw new InvalidOperationException("Lecture not found.");
            if (!_auth.CanManage(existing)) throw new UnauthorizedAccessException();
            lecture.AuthorTeacherId = existing.AuthorTeacherId;     // автора змінити не можна
            lecture.CreatedAt = existing.CreatedAt;
        }

        lecture.UpdatedAt = now;
        return _lectures.Save(lecture);
    }

    public void Delete(int id)
    {
        var existing = _lectures.GetById(id);
        if (existing is null) return;
        if (!_auth.CanManage(existing)) throw new UnauthorizedAccessException();
        _lectures.Delete(id);
    }
}
