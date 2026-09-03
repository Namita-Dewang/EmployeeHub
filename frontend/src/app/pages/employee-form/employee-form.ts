import { HttpClient } from '@angular/common/http';
import { Component, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { DesignationModel } from '../../models/Department.model';
import { createEmptyEmployee, EmployeeModel } from '../../models/Employee.model';
import { Master } from '../../services/master';

@Component({
  selector: 'app-employee-form',
  imports: [FormsModule, RouterLink],
  templateUrl: './employee-form.html',
  styleUrl: './employee-form.css',
})
export class EmployeeForm {
  private readonly http = inject(HttpClient);
  private readonly master = inject(Master);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly apiUrl = 'http://localhost:5279/api/EmployeeMaster';

  employee: EmployeeModel = createEmptyEmployee();
  designations: DesignationModel[] = [];
  isEditing = false;
  isLoading = false;
  errorMessage = '';

  constructor() {
    this.master.getAllDesignations().subscribe({ next: (items) => this.designations = items });
    this.route.queryParamMap.subscribe((params) => {
      const id = Number(params.get('id'));
      if (id > 0) {
        this.isEditing = true;
        this.isLoading = true;
        this.master.getEmployee(id).subscribe({
          next: (employee) => { this.employee = employee; this.isLoading = false; },
          error: () => { this.errorMessage = 'Unable to load the employee.'; this.isLoading = false; },
        });
      }
    });
  }

  saveEmployee() {
    this.isLoading = true;
    const request = this.isEditing
      ? this.http.put(`${this.apiUrl}/UpdateEmployee`, this.employee)
      : this.http.post(`${this.apiUrl}/AddEmployee`, this.employee);
    request.subscribe({
      next: () => this.router.navigate(['/employee-list']),
      error: () => { this.errorMessage = 'Unable to save the employee. Check the details and try again.'; this.isLoading = false; },
    });
  }

  resetForm() {
    this.employee = createEmptyEmployee();
    this.isEditing = false;
    this.errorMessage = '';
  }
}
