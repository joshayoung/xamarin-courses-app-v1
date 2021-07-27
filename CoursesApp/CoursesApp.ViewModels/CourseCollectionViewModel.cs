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
        private List<CourseViewModel> courses;
        
        public event PropertyChangedEventHandler PropertyChanged;

        public List<CourseViewModel> Courses
        {
            get => courses;
            set
            {
                courses = value;
                OnPropertyChanged();
            }
        }

        public CourseCollectionViewModel(CourseCollection courseCollection)
        {
            this.courseCollection = courseCollection;
            courseCollection.PropertyChanged += CoursesCollectionOnPropertyChanged;
            RefreshList();
        }

        private void CoursesCollectionOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(CourseCollection.Courses)) RefreshList();
        }

        private void RefreshList()
        {
            IEnumerable<CourseViewModel> courseList =
                courseCollection.Courses.Select(course => new CourseViewModel(course, courseCollection));
            
            // Setting the value will emit a property changed event:
            Courses = new List<CourseViewModel>(courseList);
        }

        public CourseViewModel NewCourseViewModel() =>
            new CourseViewModel(new Course("", 1, new List<Student>(), CourseType.Discussion), courseCollection);

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}