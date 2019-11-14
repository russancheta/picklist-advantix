import { Component, OnInit, TemplateRef } from '@angular/core';
import { Service, PickedSalesOrder, OpenSalesOrder, WebPLNo } from '../../core/api.client';
import { BsModalRef, BsModalService } from 'ngx-bootstrap';
import Swal from 'sweetalert2';
import { AuthService } from '../../shared/auth.service';
import { environment } from '../../../environments/environment';
import { ReportService } from '../../shared/report.service';

@Component({
  selector: 'app-picklist',
  templateUrl: './picklist.component.html',
  styleUrls: ['./picklist.component.scss'],
  providers: [ReportService]
})
export class PicklistComponent implements OnInit {

  pickList: PickedSalesOrder[] = [];
  checkedPL: PickedSalesOrder[] = [];
  webPLNo: WebPLNo[] = [];
  plNo = '';

  modalRef: BsModalRef;

  dtpPLDate = new Date();

  selectedPL: number = 0;

  // pagination
  page = 1;
  pageSize = 20;

  constructor(
    private apiService: Service,
    private modalService: BsModalService,
    private authService: AuthService,
    private reportService: ReportService
  ) { }

  ngOnInit() {
    this.getUpdatedSO();
  }

  getUpdatedSO() {
    this.apiService.getUpdatedSalesOrder().subscribe(response => {
      this.pickList = response;
      console.log(response);
    })
  }

  checkPicked(list: PickedSalesOrder) {
    if (this.checkedPL.includes(list)) {
      const i = this.checkedPL.indexOf(list);
      this.checkedPL.splice(i, 1);
    } else {
      this.checkedPL.push(list);
    }
    console.log(this.checkedPL);
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
      openSalesOrderList.push(openSO);
      console.log(openSO);
    });
    this.apiService.postTransaction(this.authService.getToken(), openSalesOrderList).subscribe(
      res => {
        if (res.result == 'Success') {
          this.getUpdatedSO();
          Swal.fire({
            type: 'success',
            text: 'Transaction successfully posted.'
          });
          console.log(res.result);
          console.log(res.message);
        } else {
          Swal.fire({
            type: 'error',
            title: 'Transaction was not posted.',
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
      })
    console.log(openSalesOrderList);
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
      }
    })
    this.getUpdatedSO();
  }

  showLoading() {
    Swal.fire({
      title: 'Loading',
      text: 'Please wait',
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
    this.modalRef = this.modalService.show(template);
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
    console.log(this.webPLNo);
  }

  selectedPLNo(plNo: any) {
    this.selectedPL = plNo;
  }

}