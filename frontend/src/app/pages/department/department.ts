import { HttpClient } from '@angular/common/http';
import { Component, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { DepartmentModel } from '../../models/Department.model';
import { Master } from '../../services/master';

@Component({
  selector: 'app-department',
  imports: [FormsModule],
  templateUrl: './department.html',
  styleUrl: './department.css',
})
export class Department {
  private readonly http = inject(HttpClient);
  private readonly master = inject(Master);
  private readonly apiUrl = 'http://localhost:5279/api/DepartmentMaster';

  departments: DepartmentModel[] = [];
  department: DepartmentModel = this.createEmptyDepartment();
  isEditing = false;
  isLoading = false;
  errorMessage = '';

  constructor() {
    this.loadDepartments();
  }

  loadDepartments() {
    this.isLoading = true;
    this.errorMessage = '';
    this.master.getAllDept().subscribe({
      next: (departments) => {
        this.departments = departments as DepartmentModel[];
        this.isLoading = false;
      },
      error: () => {
        this.errorMessage = 'Unable to load departments. Please try again.';
        this.isLoading = false;
      },
    });
  }

  saveDepartment() {
    const request = this.isEditing
      ? this.http.put(`${this.apiUrl}/UpdateDepartment`, this.department)
      : this.http.post(`${this.apiUrl}/AddDepartment`, this.department);

    request.subscribe({
      next: () => {
        this.resetForm();
        this.loadDepartments();
      },
      error: () => {
        this.errorMessage = 'Unable to save the department. Please try again.';
      },
    });
  }

  editDepartment(department: DepartmentModel) {
    this.department = { ...department };
    this.isEditing = true;
    this.errorMessage = '';
  }

  deleteDepartment(departmentId: number) {
    if (!confirm('Delete this department?')) {
      return;
    }

    this.http.delete(`${this.apiUrl}/DeleteDepartment/${departmentId}`).subscribe({
      next: () => this.loadDepartments(),
      error: () => {
        this.errorMessage = 'Unable to delete the department. Please try again.';
      },
    });
  }

  resetForm() {
    this.department = this.createEmptyDepartment();
    this.isEditing = false;
    this.errorMessage = '';
  }

  private createEmptyDepartment(): DepartmentModel {
    return new DepartmentModel();
  }
}
