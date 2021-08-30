﻿using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using CoursesApp.Models;

namespace CoursesApp.ViewModels
{
    public class CourseCollectionViewModel : INotifyPropertyChanged
    {
        private readonly CourseCollection courseCollection;

        public event PropertyChangedEventHandler? PropertyChanged;

        public int GetNextId => courseCollection.GetNextCourseId();

        private bool isRefreshing;

        public virtual bool IsRefreshing
        {
            get => isRefreshing;
            set
            {
                isRefreshing = value;
                OnPropertyChanged();
            }
        }

        private List<CourseViewModel> courses;

        public List<CourseViewModel> Courses
        {
            get => courses;
            set
            {
                courses = value;
                // Repopulate courses after modifying course:
                OnPropertyChanged();

                // Also Update This Value:
                OnPropertyChanged(nameof(CoursesExist));
            }
        }

        private List<StudentViewModel> students;

        public CourseCollectionViewModel(CourseCollection courseCollection)
        {
            courses = new List<CourseViewModel>();
            students = new List<StudentViewModel>();
            this.courseCollection = courseCollection;
            courseCollection.PropertyChanged += CoursesCollectionOnPropertyChanged;
            RefreshCourses();
            courseCollection.PropertyChanged += (sender, args) => PropertyChanged?.Invoke(this, args);
        }

        public bool CoursesExist => courseCollection.CoursesExist;

        public void Refresh()
        {
            IsRefreshing = true;
            courseCollection.RepopulateCourseList();
            IsRefreshing = false;
        }

        public CourseViewModel NewCourseViewModel()
        {
            return new CourseViewModel(new Course(courseCollection.GetNextCourseId()), courseCollection);
        }

        private void CoursesCollectionOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(CourseCollection.Courses)) RefreshCourses();
        }

        private void RefreshCourses()
        {
            var courseList = new List<CourseViewModel>();
            var studentList = new List<StudentViewModel>();
            courseCollection.Courses.ForEach(cs =>
            {
                studentList = cs.Students.Select(id => StudentVm(id, cs)).ToList();
                courseList.Add(CourseVm(cs, studentList));
            });
            students = studentList;
            Courses = courseList;
            courseCollection.UpdateCoursesExist();
        }

        private CourseViewModel CourseVm(Course cs, List<StudentViewModel> studentList)
        {
            var courseVm = new CourseViewModel(cs, courseCollection)
            {
                Students = studentList
            };
            return courseVm;
        }

        private StudentViewModel StudentVm(int id, Course cs)
            => new StudentViewModel(courseCollection.GetStudent(id), cs, courseCollection);
        
        private void OnPropertyChanged([CallerMemberName] string propertyName = null!)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}