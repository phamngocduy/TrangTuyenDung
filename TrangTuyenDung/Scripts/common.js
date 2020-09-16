function initDatepickup(el, options = {}) {
    return el.datepicker({
        language: 'vi',
        todayHighlight: true,
        format: "mm/dd/yyyy",
        ...options
    });
};

function initDatatable(el, options = {}) {
    return el.DataTable({
        "language": {
            "lengthMenu": "Hiển thị _MENU_ records trên mỗi trang",
            "zeroRecords": "Không tìm thấy",
            "infoEmpty": "Không có thông tin tương ứng",
            "infoFiltered": "(được lọc từ _MAX_ records)",
            "info": "Hiển thị _START_ đến _END_ trong tổng số _TOTAL_ records",
            "emptyTable": "Không có tin tuyển dụng nào",
            "sSearch": "Tìm kiếm",
            "paginate": {
                "previous": "<i class='fas fa-caret-left'></i>",
                "next": "<i class='fas fa-caret-right'></i>"
            },
        },
        "aaSorting": [], // disabled initial sort
        ...options
    });
};

function initTinymce(options = {}) {
    return tinymce.init({
        selector: 'textarea',
        plugins: "lists paste",
        language: "vi",
        menubar: false,
        toolbar: "undo redo bold italic numlist bullist",
        theme: 'silver',
        force_br_newlines: false,
        force_p_newlines: false,
        forced_root_block: '',
        paste_as_text: true,
        paste_auto_cleanup_on_paste: true,
        paste_remove_spans: true,
        paste_remove_styles: true,
        paste_retain_style_properties: false,
        invalid_elements: 'strong,em,table,thead,tr,td,tbody,tfoot,button,p,div',
        remove_trailing_brs: true,
        formats: {
            bold: { inline: 'span', classes: 'font-weight-medium' },
            italic: { inline: 'span', classes: 'font-italic' }
        },
        ...options
    });
};


//-- Format currency
//-----------------------------------
function formatCurrency(number) {
    return number.replace(/\D/g, "").replace(/\B(?=(\d{3})+(?!\d))/g, ".");
}

//-- Format currency
//-----------------------------------
function formatPhoneNumber(number) {
    return number.replace(/\D/g, "").replace(/(\d{3})(\d{3})(\d{4})/, '$1.$2.$3');
}



//-- Select list with sort options --
//-----------------------------------
//-ex: showInputField($("#Is_Full_Time"), $("#Num_FullTime"));
function searchSelect2(el, placeholder = "--Chọn--", options = {}) {
    return el.select2({
        theme: 'bootstrap4',
        dropdownAutoWidth: true,
        width: '100%',
        placeholder: placeholder,
        sorter: data => sortSelect(data),
        ...options
    });
};
function sortSelect(data) {
    return data.sort((a, b) => (a.text).localeCompare((b.text), 'vi', { sensitivity: 'base' }));
}