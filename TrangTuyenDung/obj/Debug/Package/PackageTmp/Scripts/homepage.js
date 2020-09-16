$(document).ready(function () {
    majorsSlider();
    function majorsSlider() {
        const slider = $('.major_slider .slider');
        slider.slick({
            dots: false,
            infinite: true,
            speed: 500,
            slidesToShow: 3,
            slidesToScroll: 1,
            autoplay: true,
            autoplaySpeed: 2000,
            arrows: true,
            responsive: [{
                breakpoint: 600,
                settings: {
                    slidesToShow: 2,
                    slidesToScroll: 1
                }
            },
            {
                breakpoint: 400,
                settings: {
                    arrows: false,
                    slidesToShow: 1,
                    slidesToScroll: 1
                }
            }]
        });
    }

    searchSelect2($("#city"), "--Chọn Tỉnh/TP--");
    searchSelect2($("#fac"), "--Chọn ngành--");

    function searchSelect2(el, placeholder = "--Chọn--", options) {
        return el.select2({
            theme: 'bootstrap4',
            dropdownAutoWidth: true,
            width: '100%',
            placeholder: placeholder,
            sorter: data => sortSelect(data),
            options
        });
    };
    function sortSelect(data) {
        return data.sort((a, b) => (a.text).localeCompare((b.text), 'vi', { sensitivity: 'base' }));
    }
});