export class DepartmentModel{
    departmentId: number ;
    departmentName: string ;
    isActive: boolean ;

    constructor(){
        this.departmentId =0;
        this.departmentName = '';
        this.isActive = true;
    }
}

export interface DesignationModel{
    designationId: number;
    designationName: string;
    departmentId: number;
}