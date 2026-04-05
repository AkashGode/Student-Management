import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { StudentService } from '../../services/student.service';
import { Student, StudentCreateDto } from '../../models/student.model';
import { NavbarComponent } from '../navbar/navbar.component';

type ModalMode = 'create' | 'edit' | 'view' | null;

@Component({
  selector: 'app-students',
  standalone: true,
  imports: [CommonModule, FormsModule, NavbarComponent],
  templateUrl: './students.component.html'
})
export class StudentsComponent implements OnInit {
  students: Student[] = [];
  filteredStudents: Student[] = [];
  searchQuery = '';
  isLoading = false;
  alertMessage = '';
  alertType: 'success' | 'error' | '' = '';

  // Modal
  modalMode: ModalMode = null;
  selectedStudent: Student | null = null;
  formData: StudentCreateDto = { name: '', email: '', age: 0, course: '' };
  formErrors: string[] = [];
  isSaving = false;

  // Delete
  deleteId: number | null = null;
  isDeleting = false;

  constructor(private studentService: StudentService) {}

  ngOnInit(): void { this.loadStudents(); }

  loadStudents(): void {
    this.isLoading = true;
    this.studentService.getAll().subscribe({
      next: res => {
        this.students = res.data || [];
        this.filteredStudents = [...this.students];
        this.isLoading = false;
      },
      error: () => {
        this.showAlert('Failed to load students.', 'error');
        this.isLoading = false;
      }
    });
  }

  search(): void {
    const q = this.searchQuery.toLowerCase();
    this.filteredStudents = this.students.filter(s =>
      s.name.toLowerCase().includes(q) ||
      s.email.toLowerCase().includes(q) ||
      s.course.toLowerCase().includes(q)
    );
  }

  openCreate(): void {
    this.formData = { name: '', email: '', age: 0, course: '' };
    this.formErrors = [];
    this.modalMode = 'create';
  }

  openEdit(student: Student): void {
    this.selectedStudent = student;
    this.formData = { name: student.name, email: student.email, age: student.age, course: student.course };
    this.formErrors = [];
    this.modalMode = 'edit';
  }

  openView(student: Student): void {
    this.selectedStudent = student;
    this.modalMode = 'view';
  }

  closeModal(): void {
    this.modalMode = null;
    this.selectedStudent = null;
    this.formErrors = [];
  }

  validateForm(): boolean {
    this.formErrors = [];
    if (!this.formData.name?.trim()) this.formErrors.push('Name is required.');
    if (!this.formData.email?.trim()) this.formErrors.push('Email is required.');
    else if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(this.formData.email)) this.formErrors.push('Invalid email format.');
    if (!this.formData.age || this.formData.age < 1 || this.formData.age > 120) this.formErrors.push('Age must be between 1 and 120.');
    if (!this.formData.course?.trim()) this.formErrors.push('Course is required.');
    return this.formErrors.length === 0;
  }

  saveStudent(): void {
    if (!this.validateForm()) return;
    this.isSaving = true;

    const obs = this.modalMode === 'create'
      ? this.studentService.create(this.formData)
      : this.studentService.update(this.selectedStudent!.id, this.formData);

    obs.subscribe({
      next: res => {
        this.isSaving = false;
        if (res.success) {
          this.showAlert(res.message, 'success');
          this.closeModal();
          this.loadStudents();
        } else {
          this.formErrors = res.errors || [res.message];
        }
      },
      error: err => {
        this.isSaving = false;
        this.formErrors = err?.error?.errors || [err?.error?.message || 'An error occurred.'];
      }
    });
  }

  confirmDelete(id: number): void { this.deleteId = id; }

  cancelDelete(): void { this.deleteId = null; }

  deleteStudent(): void {
    if (!this.deleteId) return;
    this.isDeleting = true;
    this.studentService.delete(this.deleteId).subscribe({
      next: res => {
        this.isDeleting = false;
        this.deleteId = null;
        if (res.success) {
          this.showAlert('Student deleted successfully.', 'success');
          this.loadStudents();
        } else {
          this.showAlert(res.message, 'error');
        }
      },
      error: () => {
        this.isDeleting = false;
        this.deleteId = null;
        this.showAlert('Failed to delete student.', 'error');
      }
    });
  }

  showAlert(msg: string, type: 'success' | 'error'): void {
    this.alertMessage = msg;
    this.alertType = type;
    setTimeout(() => { this.alertMessage = ''; this.alertType = ''; }, 4000);
  }

  get totalStudents() { return this.students.length; }
  get avgAge() {
    if (!this.students.length) return 0;
    return Math.round(this.students.reduce((s, x) => s + x.age, 0) / this.students.length);
  }
  get uniqueCourses() { return new Set(this.students.map(s => s.course)).size; }
}
