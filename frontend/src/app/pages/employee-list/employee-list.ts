import { HttpClient } from '@angular/common/http';
import { Component, inject } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { DesignationModel } from '../../models/Department.model';
import { EmployeeModel } from '../../models/Employee.model';
import { Master } from '../../services/master';

@Component({
  selector: 'app-employee-list',
  imports: [RouterLink],
  templateUrl: './employee-list.html',
  styleUrl: './employee-list.css',
})
export class EmployeeList {
  private readonly http = inject(HttpClient);
  private readonly master = inject(Master);
  private readonly router = inject(Router);
  private readonly apiUrl = 'http://localhost:5279/api/EmployeeMaster';

  employees: EmployeeModel[] = [];
  designations: DesignationModel[] = [];
  pageNumber = 1;
  totalPages = 1;
  totalRecords = 0;
  isLoading = false;
  errorMessage = '';

  constructor() {
    this.master.getAllDesignations().subscribe({ next: (items) => this.designations = items });
    this.loadEmployees();
  }

  loadEmployees() {
    this.isLoading = true;
    this.master.getEmployees(this.pageNumber).subscribe({
      next: (page) => {
        this.employees = page.data;
        this.totalPages = page.totalPages || 1;
        this.totalRecords = page.totalRecords;
        this.isLoading = false;
      },
      error: () => {
        this.errorMessage = 'Unable to load employees. Please try again.';
        this.isLoading = false;
      },
    });
  }

  editEmployee(id: number) {
    this.router.navigate(['/new-employee'], { queryParams: { id } });
  }

  deleteEmployee(id: number) {
    if (!confirm('Delete this employee?')) return;
    this.http.delete(`${this.apiUrl}/DeleteEmployee/${id}`).subscribe({
      next: () => {
        // If this was the last row on a non-first page, step back so the user
        // does not land on an empty, out-of-range page.
        if (this.employees.length === 1 && this.pageNumber > 1) {
          this.pageNumber--;
        }
        this.loadEmployees();
      },
      error: () => this.errorMessage = 'Unable to delete the employee. Please try again.',
    });
  }

  designationName(id: number) {
    return this.designations.find((designation) => designation.designationId === id)?.designationName ?? 'Unassigned';
  }

  changePage(page: number) {
    if (page < 1 || page > this.totalPages || page === this.pageNumber) return;
    this.pageNumber = page;
    this.loadEmployees();
  }
}
