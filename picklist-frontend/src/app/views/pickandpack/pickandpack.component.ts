import { Component, OnInit, TemplateRef } from '@angular/core';
import { UpdateSalesOrder, Service, OpenSalesOrder, WebPLNo, WebPlNoBranches } from '../../core/api.client';
import { BsModalRef, BsModalService, ModalOptions, getMonth, getDay } from 'ngx-bootstrap';
import { AuthService } from '../../shared/auth.service';
import { environment } from '../../../environments/environment';
import { ReportService } from '../../shared/report.service';
import { interval } from 'rxjs/internal/observable/interval';
import Swal from 'sweetalert2';
import { NgbModalOptions } from '@ng-bootstrap/ng-bootstrap';
import { flatMap, startWith } from 'rxjs/operators';

@Component({
  selector: 'app-pickandpack',
  templateUrl: './pickandpack.component.html',
  styleUrls: ['./pickandpack.component.scss']
})
export class PickAndPackComponent implements OnInit {

  openSalesOrders: OpenSalesOrder[] = [];
  checkedSO: OpenSalesOrder[] = [];

  cardCode: string;
  series: number;
  fromWarehouse: string;
  toWarehouse: string;
  comments: string;

  modalRef: BsModalRef;
  modalOption: ModalOptions = {};

  dtpPLDate = new Date();

  selectedPL: number = 0;

  webPLNo: WebPLNo[] = [];
  branchPLNo: WebPlNoBranches[] = [];
  branchName: string = '';
  plNo: number = 0;
  branchPL = [];
  remarks: string = '';
  generateFormBtnDisable = true;
  postPickListBtnDisable = false;

  datemonthNo = 0;

  refreshInterval: any;

  // pagination
  page = 1;
  pageSize = 50;

  constructor(
    private apiService: Service,
    private authService: AuthService,
    private modalService: BsModalService,
    private reportService: ReportService
  ) { }

  ngOnInit() {
    this.getOpenSalesOrders();
    this.getAllPickList(this.dtpPLDate);
    this.getDateMonthNo();
  }

  validationCheck(a: number, b: number, c: number) {
    if (a <= b && a > 0) {
      return false;
    } else {
      return true;
    };
  }

  updateList() {
    this.showLoading();
    const listupdateSo: UpdateSalesOrder[] = [];
    this.checkedSO.forEach(o => {
      const updateSO = new UpdateSalesOrder();
      updateSO.docEntry = o.docEntry;
      updateSO.docNum = o.docNum;
      updateSO.lineNum = o.lineNum;
      updateSO.pickedQty = o.qtyToPost;
      updateSO.itemCode = o.itemCode;
      updateSO.remarks = this.remarks;
      updateSO.userName = this.authService.getUserName();
      updateSO.plNo = this.plNo;
      updateSO.objType = o.objType;
      listupdateSo.push(updateSO);
    });
    this.apiService.updateSalesOrder(this.authService.getUserName(), listupdateSo).subscribe(res => {
      if (res.result == 'Success') {
        this.generateFormBtnDisable = false;
        this.postPickListBtnDisable = true;
        // this.modalRef.hide();
        // this.checkedSO = [];
        this.getOpenSalesOrders();
        Swal.fire({
          type: 'success',
          title: 'Transaction successfuly posted.',
          showConfirmButton: false,
          timer: 3500
        })
        console.log(res.message);
        console.log(res.result);
      } else {
        this.modalRef.hide();
        this.getOpenSalesOrders();
        Swal.fire({
          type: 'error',
          title: 'Transaction was not posted.',
          timer: 3500
        })
        console.log(res.message);
        console.log(res.result);
      };
    })
  }

  checkSO(obj: OpenSalesOrder) {
    if (this.checkedSO.includes(obj)) {
      const i = this.checkedSO.indexOf(obj);
      this.checkedSO.splice(i, 1);
    } else {
      this.checkedSO.push(obj);
    }
    console.table(this.checkedSO);
    console.log(this.checkedSO.length);
  }

  getOpenSalesOrders() {
    this.showLoading();
    this.apiService.getOpenSalesOrder().subscribe(response => {
      this.openSalesOrders = response;
      Swal.close();
      /*this.refreshInterval = interval(6000).pipe(
        startWith(0),
        flatMap(() => this.apiService.getOpenSalesOrder())).subscribe(response => {
          this.openSalesOrders = response;
          Swal.close();
        },
          error => {
            Swal.close();
            console.log('HTTP error', error);
          }
        );*/
      }
    );
  }

  viewReport() {
    console.log(this.plNo.toString());
    const ePLNO = this.reportService.setEncryptedData(this.plNo.toString());
    window.open(
      environment.REPORT_BASE_URL + '/Report/PickList?'
      + 'pickListNo=' + ePLNO, '_blank'
    );
    console.log(this.plNo);
  }

  openModal(template: TemplateRef<any>) {
    this.generateFormBtnDisable = true;
    this.apiService.getPicklistNo().subscribe(
      res => {
        this.plNo = res.pickListNum;
      }
    );
    this.remarks = '';
    this.modalRef = this.modalService.show(template, { ignoreBackdropClick: true, keyboard: false, class: 'modal-xl' });
  }

  pickListModal(pickandpack: TemplateRef<any>) {
    this.modalRef = this.modalService.show(pickandpack);
  }

  getAllPickList(date: Date) {
    this.apiService.getWebPLNo(date).subscribe(res => { this.webPLNo = res; });
    this.plNo = this.webPLNo[0].plNo;
    console.log(this.plNo);
  }

  // getBranchPLNo() {
  //   this.apiService.getWebPl('AUTOMATIC CENTRE - North Edsa').subscribe(res => {
  //     this.branchPL = res
  //     console.log(res);
  //   })
  // }

  findPLNo() {
    this.getAllPickList(this.dtpPLDate);
     if (this.webPLNo.length !== 0) {
       this.plNo = this.webPLNo[0].plNo;
     }
  }

  selectedPLNo(plNo: number) {
    console.log(plNo)
    this.plNo = plNo;
  }

  getDateMonthNo() {
    let dayNo = this.dtpPLDate.getDate();
    let monthNo = getMonth(this.dtpPLDate) + 1;
  }

  closeModal() {
    this.modalRef.hide();
    this.checkedSO = [];
    this.getOpenSalesOrders();
  }

  showLoading() {
    Swal.fire({
      title: 'Loading',
      text: 'Please wait',
      imageUrl: 'data:image/gif;base64,R0lGODlhEAAQAPIAAP///wAAAMLCwkJCQgAAAGJiYoKCgpKSkiH/C05FVFNDQVBFMi4wAwEAAAAh/hpDcmVhdGVkIHdpdGggYWpheGxvYWQuaW5mbwAh+QQJCgAAACwAAAAAEAAQAAADMwi63P4wyklrE2MIOggZnAdOmGYJRbExwroUmcG2LmDEwnHQLVsYOd2mBzkYDAdKa+dIAAAh+QQJCgAAACwAAAAAEAAQAAADNAi63P5OjCEgG4QMu7DmikRxQlFUYDEZIGBMRVsaqHwctXXf7WEYB4Ag1xjihkMZsiUkKhIAIfkECQoAAAAsAAAAABAAEAAAAzYIujIjK8pByJDMlFYvBoVjHA70GU7xSUJhmKtwHPAKzLO9HMaoKwJZ7Rf8AYPDDzKpZBqfvwQAIfkECQoAAAAsAAAAABAAEAAAAzMIumIlK8oyhpHsnFZfhYumCYUhDAQxRIdhHBGqRoKw0R8DYlJd8z0fMDgsGo/IpHI5TAAAIfkECQoAAAAsAAAAABAAEAAAAzIIunInK0rnZBTwGPNMgQwmdsNgXGJUlIWEuR5oWUIpz8pAEAMe6TwfwyYsGo/IpFKSAAAh+QQJCgAAACwAAAAAEAAQAAADMwi6IMKQORfjdOe82p4wGccc4CEuQradylesojEMBgsUc2G7sDX3lQGBMLAJibufbSlKAAAh+QQJCgAAACwAAAAAEAAQAAADMgi63P7wCRHZnFVdmgHu2nFwlWCI3WGc3TSWhUFGxTAUkGCbtgENBMJAEJsxgMLWzpEAACH5BAkKAAAALAAAAAAQABAAAAMyCLrc/jDKSatlQtScKdceCAjDII7HcQ4EMTCpyrCuUBjCYRgHVtqlAiB1YhiCnlsRkAAAOwAAAAAAAAAAAA==',
      showConfirmButton: false,
      allowOutsideClick: false
    });
  }

  showAlert() {
    Swal.fire({
      title: 'Are you sure?',
      text: "Do you want to close this window",
      type: 'warning',
      showCancelButton: true,
      confirmButtonColor: '#3085d6',
      cancelButtonColor: '#d33',
      confirmButtonText: 'Yes!'
    }).then((result) => {
      if (result.value) {
        this.closeModal();
      }
    });
  }

  /*
  postSAP() {
    this.apiService.createOPKL(this.checkedSO).subscribe(res => {
      if(res.result === 'success'){
        console.log(res.message);
      } else {
        console.log(res.message);
      }
    });

    // const checkedSOSorted: OpenSalesOrder[] = this.checkedSO.sort((a, b) => (a.docNum > b.docNum) ? 1 : -1);
    // let container: OpenSalesOrder[] = [];
    // for (let i = 0; i < checkedSOSorted.length; i++) {
    //   let comparisonIndex = 0;
    //   if (i < checkedSOSorted.length - 1) {
    //     comparisonIndex = i + 1;
    //   } else {
    //     comparisonIndex = i;
    //   }

    //   container.push(checkedSOSorted[i]);
    //   if (checkedSOSorted[i].docNum != checkedSOSorted[comparisonIndex].docNum) {
    //     if (container[0].soType === 'Consignment') {
    //       // this.postConsignment(container);
    //       console.log('PUSH CONSIGNMENT');
    //       console.log(container);
    //     } else {
    //       console.log('PUSH Outright');
    //       // console.log(container);
    //       this.postOutright(container);
    //     }
    //     container = [];
    //   }

    //   if (i + 1 == checkedSOSorted.length) {
        
    //     if (container[0].soType === 'Consignment') {
    //       console.log('PUSH CONSIGNMENT');
    //       console.log(container);
    //       this.postConsignment(container);
    //     } else {
    //       console.log('PUSH Outright');
    //       console.log(container);
    //       this.postOutright(container);
    //     }
    //     container = [];
    //   }
    //   //this.getOpenSalesOrders();
    // }
  }
  
  postOutright(data: OpenSalesOrder[]) {

    // header
    const odlnViewModel = new ODLNViewModel();
    odlnViewModel.cardCode = data[0].cardCode;
    odlnViewModel.comments = 'Document posted through Pick and Pack Addon web app.';

    // rows
    const odlnContentViewModel: ODLNContentViewModel[] = [];
    data.forEach(o => {
      const odlnInfo = new ODLNContentViewModel();
      odlnInfo.itemCode = o.itemCode;
      odlnInfo.quantity = o.qtyToPost;
      odlnInfo.baseEntry = o.docEntry;
      odlnInfo.baseLine = o.lineNum;
      odlnInfo.baseType = Number(o.objType);
      odlnContentViewModel.push(odlnInfo);
    });

    odlnViewModel.odlnContent = odlnContentViewModel;
    console.log(odlnViewModel);

    // post data
    this.apiService.createODLN(odlnViewModel)
      .subscribe(res => {
        if (res.result == 'Success') {
          alert(res.message);
        }
      });
  }

  postConsignment(data: OpenSalesOrder[]) {

    // header
    const owtrViewModel = new OWTRViewModel();
    owtrViewModel.series = 147;
    owtrViewModel.cardCode = data[0].cardCode;
    owtrViewModel.itType = 'SALES';
    owtrViewModel.fromWarehouse = "1"
    owtrViewModel.toWarehouse = data[0].whseBranch;
    owtrViewModel.comments = 'Document posted through Pick and Pack Addon web app.';

    // rows
    const owtrContentViewModel: OWTRContentViewModel[] = [];
    data.forEach(o => {
      const owtrInfo = new OWTRContentViewModel();
      owtrInfo.itemCode = o.itemCode;
      owtrInfo.quantity = o.qtyToPost;
      owtrContentViewModel.push(owtrInfo);
    });

    owtrViewModel.owtrContent = owtrContentViewModel;
    console.log(owtrViewModel);

    // post data
    this.apiService.createOWTR(owtrViewModel).subscribe(response => {
      if (response.result == 'Success') {
        alert(response.message);
      }
    });
  } */

}
