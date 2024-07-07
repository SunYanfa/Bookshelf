import { useEffect, useState } from 'react';
import { Button, Modal, Flex, Tag } from 'antd';
import translations from '../language/ch';
import { GrLocation, GrUserManager } from "react-icons/gr";
import {
    AiOutlineMenu, AiOutlineProduct, AiOutlineSignature, AiOutlineBulb, AiOutlineApartment, AiOutlineAreaChart, AiOutlineEdit, AiOutlineHeart
} from "react-icons/ai";





const ModalBookshelf = ({ open, setOpen, modaldata, setmodaldata }) => {
    const [loading, setLoading] = useState(false);
    const showModal = () => {
        setOpen(true);
    };
    const handleOk = () => {
        setLoading(true);
        setTimeout(() => {
            setLoading(false);
            setOpen(false);
        }, 3000);
    };
    const handleCancel = () => {
        setOpen(false);
    };
    console.log(modaldata);

    return (
        <>
            <Modal
                open={open}
                title={modaldata.novelName}
                onOk={handleOk}
                onCancel={handleCancel}
                footer={[
                    <Button key="back" onClick={handleCancel}>
                        Return
                    </Button>,
                    <Button key="submit" type="primary" loading={loading} onClick={handleOk}>
                        Submit
                    </Button>,
                    <Button
                        key="link"
                        href="https://google.com"
                        type="primary"
                        loading={loading}
                        onClick={handleOk}
                    >
                        Search on Google
                    </Button>,
                ]}
            >
                <p><AiOutlineApartment /> {translations.authorName}: {modaldata?.authorName || '未知作者'}</p>
                <p><AiOutlineApartment /> {translations.novelClass}: {modaldata?.novelClass || '未知类别'}</p>
                <Flex gap="4px 0" wrap>
                    {modaldata?.novelTags?.length > 0 ? (
                        modaldata.novelTags.map((tag) => (
                            <Tag color="green" key={tag}> {tag.toUpperCase()} </Tag>
                        ))
                    ) : (
                        <p>没有标签</p>
                    )}
                </Flex>
                <p><AiOutlineProduct /> {translations.novelIntro}: {modaldata?.novelIntro || '暂无简介'}</p>
                <p><AiOutlineBulb /> {translations.novelIntroShort}: {modaldata?.novelIntroShort || '暂无短简介'}</p>
                <p><AiOutlineHeart /> {translations.novelbefavoritedcount}: {modaldata?.novelbefavoritedcount || '0'}</p>
                <p><AiOutlineEdit /> {translations.novelSize}: {modaldata?.novelSize || '未知字数'}</p>
                <p><AiOutlineAreaChart /> {translations.novelCover}: <img src={modaldata?.novelCover} alt="小说封面" /></p>
                <p><GrLocation /> {translations.novelStep}: {modaldata?.novelStep || '未知进度'}</p>
                <p><AiOutlineProduct /> {translations.isComplete}: {modaldata?.isComplete ? '是' : '否'}</p>
                <p><AiOutlineProduct /> {translations.novelDate}: {new Date(modaldata?.novelData).toLocaleString()}</p>
            </Modal>
        </>
    );
};
export default ModalBookshelf;
