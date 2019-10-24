import { Component, OnInit, TemplateRef } from '@angular/core';
import { Service, SalesOrderMonitoring, SalesOrderMonitoringDetails, OpenSalesOrder } from '../../core/api.client';
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
  checkSOD: number[] = [];
  docEntryList = [];

  soNo = '';

  modalRef: BsModalRef;

  constructor(
    private apiService: Service,
    private modalService: BsModalService,
    private authService: AuthService
  ) { }

  ngOnInit() {
    this.getSalesOrderMonitoring();
  }

  getSalesOrderMonitoring() {
    this.apiService.getSalesOrderMonitoring().subscribe(res => {
      this.soMonitoring = res;
      console.table(res);
    })
  }

  getSOMonitDetails(docnum: number) {
    this.apiService.getSalesOrderMonitoringDetails(docnum).subscribe(res => {
      this.soMonitoringDetails = res;
    })
  }

  checkedSalesOrder(docEntry: number) {
    if (this.checkSOD.includes(docEntry)) {
      const i = this.checkSOD.indexOf(docEntry);
      this.checkSOD.splice(i, 1);
    } else {
      this.checkSOD.push(docEntry);
    }
    console.table(this.checkSOD);
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
      showConfirmButton: false,
      allowOutsideClick: false
    })
  }

  openModal(details: TemplateRef<any>, docNum: number) {
    this.modalRef = this.modalService.show(details);
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