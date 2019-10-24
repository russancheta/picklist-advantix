import { Component, OnInit, TemplateRef } from '@angular/core';
import { UpdateSalesOrder, Service, OpenSalesOrder } from '../../core/api.client';
import { BsModalRef, BsModalService } from 'ngx-bootstrap';
import { AuthService } from '../../shared/auth.service';
import Swal from 'sweetalert2';

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

  plNo: number = 0;
  remarks: string = '';

  constructor(
    private apiService: Service,
    private authService: AuthService,
    private modalService: BsModalService
  ) { }

  ngOnInit() {
    this.getOpenSalesOrders();
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
      updateSO.lineNum = o.lineNum;
      updateSO.pickedQty = o.qtyToPost;
      updateSO.itemCode = o.itemCode;
      updateSO.remarks = this.remarks;
      updateSO.plNo = this.plNo;
      updateSO.objType = o.objType;
      listupdateSo.push(updateSO);
    });
    this.apiService.updateSalesOrder(this.authService.getUserName() ,listupdateSo).subscribe(res => {
      if (res.result == 'Success') {
        this.modalRef.hide();
        this.checkedSO = [];
        this.getOpenSalesOrders();
        Swal.fire({
          type: 'success',
          title: 'Transaction successfuly posted.',
          showConfirmButton: false,
          timer: 2500
        })
        console.log(res.message);
        console.log(res.result);
      } else {
        this.modalRef.hide();
        this.getOpenSalesOrders();
        Swal.fire({
          type: 'error',
          title: 'Transaction was not posted.',
          timer: 2500
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
  }

  getOpenSalesOrders() {
    this.apiService.getOpenSalesOrder().subscribe(response => {
      this.openSalesOrders = response;
      console.table(response);
    });
  }

  openModal(template: TemplateRef<any>) {
    this.apiService.getPicklistNo().subscribe(
      res => {
        this.plNo = res.pickListNum;
      }
    );
    this.remarks = '';
    this.modalRef = this.modalService.show(template);
  }

  closeModal() {
    this.modalRef.hide();
  }

  showLoading() {
    Swal.fire({
      title: 'Loading',
      text: 'Please wait',
      showConfirmButton: false,
      allowOutsideClick: false
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
  */

  /*
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
