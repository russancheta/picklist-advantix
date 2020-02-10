import { Component, OnInit, TemplateRef } from '@angular/core';
import { Service, SalesOrderMonitoring, SalesOrderMonitoringDetails, ForClosingViewModel } from '../../core/api.client';
import { BsModalRef, BsModalService } from 'ngx-bootstrap';
import { AuthService } from '../../shared/auth.service';
import Swal from 'sweetalert2';

@Component({
  selector: 'app-openso',
  templateUrl: './openso.component.html',
  styleUrls: ['./openso.component.scss']
})
export class OpenSOComponent implements OnInit {

  soMonitoring: SalesOrderMonitoring[] = [];
  soMonitoringDetails: SalesOrderMonitoringDetails[] = [];
  checkedSO: SalesOrderMonitoring[] = [];
  checkSOD: ForClosingViewModel[] = [];
  docEntryList = [];

  soNo = '';

  modalRef: BsModalRef;

  remarks: string = '';

  button = true;

  // pagination
  page = 1;
  pageSize = 50;

  constructor(
    private apiService: Service,
    private modalService: BsModalService,
    private authService: AuthService
  ) { }

  ngOnInit() {
    this.getSalesOrderMonitoring();
  }

  getSalesOrderMonitoring() {
    this.showLoading();
    this.apiService.getSalesOrderMonitoring().subscribe(res => {
      this.soMonitoring = res;
      Swal.close();
    })
  }

  getSOMonitDetails(docnum: number) {
    this.apiService.getSalesOrderMonitoringDetails(docnum).subscribe(res => {
      this.soMonitoringDetails = res;
    })
  }

  // checkedSalesOrder(docEntry: number, type: string) {
  //   const o = new ForClosingViewModel();
  //   o.docEntry = docEntry;
  //   o.type = type;
  //   if (this.checkSOD.includes(o)) {
  //     const i = this.checkSOD.indexOf(o);
  //     this.checkSOD.splice(i, 1);
  //   } else {
  //     this.checkSOD.push(o);
  //   }
  //   console.log(o);
  // }

  checkedSalesOrder(docEntry: number, type: string, remarks: string) {
    const o = new SalesOrderMonitoring
    o.docEntry = docEntry;
    o.type = type;
    o.remarks = remarks;
    if (this.checkSOD.includes(o)) {
      const i = this.checkSOD.indexOf(o);
      this.checkSOD.splice(i, 1);
    } else {
      this.checkSOD.push(o);
    }
    console.log(o);
  }

  closeSalesOrder() {
    this.showLoading();
    this.apiService.closeSO(this.authService.getToken(), this.checkSOD).subscribe(
      res => {
        if (res.result == 'Success') {
          this.getSalesOrderMonitoring();
          Swal.fire({
            type: 'success',
            text: 'Document successfully closed.'
          });
          console.log(res.result);
          console.log(res.message);
        } else {
          Swal.fire({
            type: 'error',
            title: 'Closing of document failed.',
            text: res.message
          })
          console.log(res.result);
          console.log(res.message);
        }
      }, error => {
        Swal.fire({
          type: 'error',
          text: 'Oops. Something went wrong.'
        });
        Swal.close();
      }
    )
  }

  showLoading() {
    Swal.fire({
      title: 'Loading',
      text: 'Please wait',
      imageUrl: 'data:image/gif;base64,R0lGODlhEAAQAPIAAP///wAAAMLCwkJCQgAAAGJiYoKCgpKSkiH/C05FVFNDQVBFMi4wAwEAAAAh/hpDcmVhdGVkIHdpdGggYWpheGxvYWQuaW5mbwAh+QQJCgAAACwAAAAAEAAQAAADMwi63P4wyklrE2MIOggZnAdOmGYJRbExwroUmcG2LmDEwnHQLVsYOd2mBzkYDAdKa+dIAAAh+QQJCgAAACwAAAAAEAAQAAADNAi63P5OjCEgG4QMu7DmikRxQlFUYDEZIGBMRVsaqHwctXXf7WEYB4Ag1xjihkMZsiUkKhIAIfkECQoAAAAsAAAAABAAEAAAAzYIujIjK8pByJDMlFYvBoVjHA70GU7xSUJhmKtwHPAKzLO9HMaoKwJZ7Rf8AYPDDzKpZBqfvwQAIfkECQoAAAAsAAAAABAAEAAAAzMIumIlK8oyhpHsnFZfhYumCYUhDAQxRIdhHBGqRoKw0R8DYlJd8z0fMDgsGo/IpHI5TAAAIfkECQoAAAAsAAAAABAAEAAAAzIIunInK0rnZBTwGPNMgQwmdsNgXGJUlIWEuR5oWUIpz8pAEAMe6TwfwyYsGo/IpFKSAAAh+QQJCgAAACwAAAAAEAAQAAADMwi6IMKQORfjdOe82p4wGccc4CEuQradylesojEMBgsUc2G7sDX3lQGBMLAJibufbSlKAAAh+QQJCgAAACwAAAAAEAAQAAADMgi63P7wCRHZnFVdmgHu2nFwlWCI3WGc3TSWhUFGxTAUkGCbtgENBMJAEJsxgMLWzpEAACH5BAkKAAAALAAAAAAQABAAAAMyCLrc/jDKSatlQtScKdceCAjDII7HcQ4EMTCpyrCuUBjCYRgHVtqlAiB1YhiCnlsRkAAAOwAAAAAAAAAAAA==',
      showConfirmButton: false,
      allowOutsideClick: false
    })
  }

  openModal(details: TemplateRef<any>, docNum: number) {
    this.modalRef = this.modalService.show(details, { class: 'modal-lg' });
    this.getSOMonitDetails(docNum);
    this.soNo = docNum.toString();
  }

  closeModal() {
    this.modalRef.hide();
  }

  closeConfirm() {
    Swal.fire({
      title: 'Do you want to close the selected document?',
      type: 'question',
      showCancelButton: true,
      confirmButtonColor: '#3085d6',
      cancelButtonColor: '#d33',
      confirmButtonText: 'Confirm'
    }).then((result) => {
      if (result.value) {
        this.closeSalesOrder();
        console.log('testing close confirm');
      }
    });
  }

}
