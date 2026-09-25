  //Paging

    var reachedEOF = false;
    var api_response_data = null;
    var lastRow = null;
    var totalRecordsCount = 0;
    var pageSize = 50;
    var currentPageNumber = 1;
    var fetchingDataFromApi = false;
    var currentTab = "consolidated";
    var pagerContainerId = '#pager_container';
    var dataTableId = '';
    //Paging

  


    function pager_getPageCount() {
        var pageCount = Math.floor(totalRecordsCount / pageSize);
        if (totalRecordsCount % pageSize > 0) {
            pageCount++;
        }
        return pageCount;
    }
    function pager_setPageCount(newPageSize) {
        pageSize = newPageSize;

        $(pagerContainerId + ' #pager_lbl_pagecount').text(pager_getPageCount());
    }

    function pager_onPageChanged() {
        var prevButton = $(pagerContainerId + ' #pager_btn_previous');
        var nextButton = $(pagerContainerId + ' #pager_btn_next');

        if (pageSize >= totalRecordsCount) {
            prevButton.prop('disabled', true);
            prevButton.addClass('disabled');

            nextButton.prop('disabled', true);
            nextButton.addClass('disabled');
        }
        else if (currentPageNumber <= 1) {
            prevButton.prop('disabled', true);
            prevButton.addClass('disabled');

            nextButton.prop('disabled', false);
            nextButton.removeClass('disabled');
            reachedEOF = false;
        }
        else if (currentPageNumber >= pager_getPageCount()) {
            prevButton.prop('disabled', false);
            prevButton.removeClass('disabled');

            nextButton.prop('disabled', true);
            nextButton.addClass('disabled');
        }
        else if (pager_getPageCount() == 1) {
            prevButton.prop('disabled', true);
            prevButton.addClass('disabled');

            nextButton.prop('disabled', true);
            nextButton.addClass('disabled');
        }
        else {
            prevButton.prop('disabled', false);
            prevButton.removeClass('disabled');

            nextButton.prop('disabled', false);
            nextButton.removeClass('disabled');
            reachedEOF = false;
        }

        //var startRowCount = (pageSize * currentPageNumber) - pageSize + 1;
        //var endRowCount = $('#tblClientRequest').DataTable().rows().count() + startRowCount - 1;
        //var pagingInfo = 'Showing ' + startRowCount + ' to ' + endRowCount + ' of ' + totalRecordsCount + ' entries';
        //$('#tblClientRequest_info').text(pagingInfo);
    }

    function pager_setCurrentPageNumber(pageNumber) {
        currentPageNumber = pageNumber;
        $(pagerContainerId + ' #pager_txt_currentpage').val(pageNumber);
    }

    function pager_gotoNextPage(btn) {
        pager_setCurrentPageNumber(currentPageNumber + 1);
         
            fill_pagination(pager_onPageChanged);
        
    }

    function pager_gotoPage(txtBox) {
        var seekPageNumber = parseInt(txtBox.value, 10);
        if (seekPageNumber > 0 && seekPageNumber <= pager_getPageCount()) {
            pager_setCurrentPageNumber(seekPageNumber);
        }
        else {
            txtBox.value = txtBox.oldValue;
        }
         
            fill_pagination(pager_onPageChanged);
        
    }

    function pager_gotoPreviousPage(btn) {
        pager_setCurrentPageNumber(currentPageNumber - 1);

        pager_onPageChanged();
         
            fill_pagination();
        
    }

    function pager_pageSizeChanged(ddl) {
        reachedEOF = false;

        pager_setPageCount(ddl.value);

        pager_setCurrentPageNumber(1);
             fill_pagination(pager_onPageChanged);
     }
