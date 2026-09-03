  import { HttpClient } from '@angular/common/http';
  import { Component, inject } from '@angular/core';
  import { FormsModule } from '@angular/forms';
  import { DepartmentModel, DesignationModel } from '../../models/Department.model';
  import { Master } from '../../services/master';

@Component({
  selector: 'app-designation',
  imports: [FormsModule],
  templateUrl: './designation.html',
  styleUrl: './designation.css',
})
export class Designation {
  private readonly http = inject(HttpClient);
  private readonly master = inject(Master);
  private readonly apiUrl = 'http://localhost:5279/api/DesignationMaster';

  designations: DesignationModel[] = [];
  departments: DepartmentModel[] = [];
  designation: DesignationModel = this.createEmptyDesignation();
  isEditing = false;
  isLoading = false;
  errorMessage = '';

  constructor() {
    this.loadDesignations();
    this.master.getAllDept().subscribe({
      next: (departments) => this.departments = departments,
      error: () => this.errorMessage = 'Unable to load departments for designations.',
    });
  }

  loadDesignations() {
    this.isLoading = true;
    this.master.getAllDesignations().subscribe({
      next: (designations) => {
        this.designations = designations;
        this.isLoading = false;
      },
      error: () => {
        this.errorMessage = 'Unable to load designations. Please try again.';
        this.isLoading = false;
      },
    });
  }

  saveDesignation() {
    const request = this.isEditing
      ? this.http.put(`${this.apiUrl}/UpdateDesignation`, this.designation)
      : this.http.post(`${this.apiUrl}/AddDesignation`, this.designation);

    request.subscribe({
      next: () => {
        this.resetForm();
        this.loadDesignations();
      },
      error: () => this.errorMessage = 'Unable to save the designation. Please try again.',
    });
  }

  editDesignation(designation: DesignationModel) {
    this.designation = { ...designation };
    this.isEditing = true;
    this.errorMessage = '';
  }

  deleteDesignation(id: number) {
    if (!confirm('Delete this designation?')) return;

    this.http.delete(`${this.apiUrl}/DeleteDesignation/${id}`).subscribe({
      next: () => this.loadDesignations(),
      error: () => this.errorMessage = 'Unable to delete the designation. Please try again.',
    });
  }

  resetForm() {
    this.designation = this.createEmptyDesignation();
    this.isEditing = false;
    this.errorMessage = '';
  }

  departmentName(id: number) {
    return this.departments.find((department) => department.departmentId === id)?.departmentName ?? 'Unassigned';
  }

  private createEmptyDesignation(): DesignationModel {
    return { designationId: 0, designationName: '', departmentId: 0 };
  }

}
