import { Component, OnInit, TemplateRef } from '@angular/core';
import { Service, PickedSalesOrder, OpenSalesOrder, WebPLNo } from '../../core/api.client';
import { BsModalRef, BsModalService, getDay } from 'ngx-bootstrap';
import Swal from 'sweetalert2';
import { AuthService } from '../../shared/auth.service';
import { environment } from '../../../environments/environment';
import { ReportService } from '../../shared/report.service';
import { DatePipe } from '@angular/common';

@Component({
  selector: 'app-picklist',
  templateUrl: './picklist.component.html',
  styleUrls: ['./picklist.component.scss'],
  providers: [ReportService, DatePipe]
})
export class PicklistComponent implements OnInit {

  pickList: PickedSalesOrder[] = [];
  checkedPL: PickedSalesOrder[] = [];
  webPLNo: WebPLNo[] = [];
  plNo = '';

  modalRef: BsModalRef;

  dtpPLDate = new Date();

  selectedPL: number = 0;

  postTransactionBtn = false;

  // pagination
  page = 1;
  pageSize = 50;

  constructor(
    private apiService: Service,
    private modalService: BsModalService,
    private authService: AuthService,
    private reportService: ReportService,
    private datePipe: DatePipe
  ) { }

  ngOnInit() {
    this.getUpdatedSO();
  }

  getUpdatedSO() {
    this.showLoading();
    this.apiService.getUpdatedSalesOrder().subscribe(response => {
      this.pickList = response;
      Swal.close();
      console.table(response);
    })
  }

  checkPicked(list: PickedSalesOrder) {
    if (this.checkedPL.includes(list)) {
      const i = this.checkedPL.indexOf(list);
      this.checkedPL.splice(i, 1);
    } else {
      this.checkedPL.push(list);
    }
  }

  postTransaction() {
    this.showLoading();
    const openSalesOrderList: OpenSalesOrder[] = [];

    this.checkedPL.forEach(o => {
      const openSO = new OpenSalesOrder();
      openSO.docEntry = o.docEntry;
      openSO.docNum = o.docNum;
      openSO.cardCode = o.cardCode;
      openSO.cardName = o.cardName;
      openSO.soType = o.soType;
      openSO.itemCode = o.itemCode;
      openSO.qtyToPost = o.qtyToPost;
      openSO.whsCode = o.whsCode;
      openSO.whseBranch = o.whseBranch;
      openSO.docEntry = o.docEntry;
      openSO.lineNum = o.lineNum;
      openSO.objType = o.objType;
      openSO.useBaseUnits = o.useBaseUnits;
      openSO.docDate = new Date(this.datePipe.transform(o.docDate, 'yyyy-MM-dd'));
      openSO.soType = o.soType;
      openSO.poNo = o.poNo;
      openSO.delStat = o.delStatus;
      openSO.cancelDate = new Date(this.datePipe.transform(o.cancelDate, 'yyyy-MM-dd'));
      openSO.shipTo = o.shipTo;
      openSalesOrderList.push(openSO);
    });
    this.apiService.postTransaction(this.authService.getToken(), openSalesOrderList).subscribe(
      res => {
        if (res.result == 'Success') {
          this.getUpdatedSO();
          Swal.fire({
            type: 'success',
            text: 'Transaction successfully posted.'
          });
        } else {
          Swal.fire({
            type: 'error',
            title: 'Transaction was not posted.',
            text: res.message
          })
        }
      }, error => {
        Swal.fire({
          type: 'error',
          text: 'Oops. Something went wrong.'
        });
      });
  }

  validationCheck(a: number, b: number, c: number) {
    if (a <= b && a > 0 && a <= c) {
      return false;
    } else {
      return true;
    };
  }

  transactionConfirm() {
    Swal.fire({
      title: 'Do you want to post the selected documents?',
      type: 'question',
      showCancelButton: true,
      confirmButtonColor: '#3085d6',
      cancelButtonColor: '#d33',
      confirmButtonText: 'Confirm'
    }).then((result) => {
      if (result.value) {
        this.postTransaction();
        this.postTransactionBtn = true;
      }
    })
    // this.getUpdatedSO();
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

  viewReport() {
    const ePLNO = this.reportService.setEncryptedData(this.selectedPL.toString());
    window.open(
      environment.REPORT_BASE_URL + '/Report/PickList?'
      + 'pickListNo=' + ePLNO, '_blank'
    );
  }

  openModal(template: TemplateRef<any>) {
    this.modalRef = this.modalService.show(template, { ignoreBackdropClick: true, keyboard: false, class: 'modal-xl' });
  }

  closeModal() {
    this.modalRef.hide();
  }

  checkPlNo() {
    console.log(this.plNo);
  }

  getAllPickList(date: Date) {
    this.apiService.getWebPLNo(date).subscribe(res => { this.webPLNo = res; });
  }

  findPLNo() {
    this.getAllPickList(this.dtpPLDate);
  }

  selectedPLNo(plNo: any) {
    this.selectedPL = plNo;
  }

}