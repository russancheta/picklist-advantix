import { Component, OnInit, TemplateRef } from '@angular/core';
import { Service, PickedSalesOrder, OpenSalesOrder } from '../../core/api.client';
import { BsModalRef, BsModalService } from 'ngx-bootstrap';
import Swal from 'sweetalert2';
import { AuthService } from '../../shared/auth.service';
@Component({
  selector: 'app-picklist',
  templateUrl: './picklist.component.html',
  styleUrls: ['./picklist.component.scss']
})
export class PicklistComponent implements OnInit {

  pickList: PickedSalesOrder[] = [];
  checkedPL: PickedSalesOrder[] = [];

  modalRef: BsModalRef;

  constructor(
    private apiService: Service,
    private modalService: BsModalService,
    private authService: AuthService
  ) { }

  ngOnInit() {
    this.getUpdatedSO();
  }

  getUpdatedSO() {
    this.apiService.getUpdatedSalesOrder().subscribe(response => {
      this.pickList = response;
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
    console.table(this.checkedPL);
  }

  postTransaction() {
    this.showLoading();
    const openSalesOrderList: OpenSalesOrder[] = [];
    this.checkedPL.forEach(o => {
      const openSO = new OpenSalesOrder();
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

}
