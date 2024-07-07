import { useEffect, useState } from 'react';
import { Space, Table, Tag, Button, message, Upload } from 'antd';
import {
    CheckOutlined, CloseOutlined,
    CrownOutlined, LineOutlined,
    UploadOutlined,
    LoadingOutlined,
    SettingFilled,
    SmileOutlined,
    SyncOutlined,
} from '@ant-design/icons';
import ModalBookshelf from './Component/ModalComponent';
//import './App.css';

function Bookshelf() {  
    const [books, setBooks] = useState([]);  // 将初始状态设为空数组
    const [modaldata, setmodaldata] = useState([]);

    const [loading, setLoading] = useState(false);
    const [open, setOpen] = useState(false);

    const [tableParams, setTableParams] = useState({
        pagination: {
            current: 1,
            pageSize: 10,
        },
    });

    const getRandomuserParams = (params) => ({
        results: params.pagination?.pageSize,
        page: params.pagination?.current,
        ...params,
    });

    const showModal = (record) => {
        setOpen(true);
        setmodaldata(record);
    };

    useEffect(() => {
        populateBookData();
    }, []);

    const columns = [
        { title: 'Novel Name', dataIndex: 'novelName', key: 'novelName'},
        { title: 'Author Name', dataIndex: 'authorName', key: 'authorName'},
        { title: 'Novel Class', dataIndex: 'novelClass', key: 'novelClass'},
        { title: 'Novel Tags', dataIndex: 'novelTags', key: 'novelTags',
            render: (_, { novelTags }) => (
                <>
                    {novelTags.map((tag) => {
                        let color = tag.length > 5 ? 'geekblue' : 'green';
                        return ( <Tag color={color} key={tag}> {tag.toUpperCase()} </Tag> );
                    })}
                </>
            ),
        },
        //{
        //    title: 'Novel Intro Short',
        //    dataIndex: 'novelIntroShort',
        //    key: 'novelIntroShort',
        //},
        { title: 'Favorites Count', dataIndex: 'novelbefavoritedcount', key: 'novelbefavoritedcount' },
        { title: 'Is Saved', dataIndex: 'isSaved', key: 'isSaved',
            render: (isSaved) => ( isSaved ? <CheckOutlined /> : <CloseOutlined /> ),
        },
        { title: 'Novel Step', dataIndex: 'novelStep', key: 'novelStep',
            render: (novelStep) => ( (novelStep == 2) ? <CrownOutlined /> : <LineOutlined /> ),
        },
        { title: 'Novel Year', dataIndex: 'novelYear', key: 'novelYear' },
        { title: 'Action', key: 'action', render: (_, record) => (
                <Space size="middle">
                    <a>Invite {record.name}</a>
                    <Button onClick={() => showModal(record)}>Delete</Button>
                </Space>
            ),
        }
    ];

    
    const contents = books.length === 0  // 检查 books 长度是否为 0
        ? <p><em>Loading... Please refresh once the ASP.NET backend has started. See <a href="https://aka.ms/jspsintegrationreact">https://aka.ms/jspsintegrationreact</a> for more details.</em></p>
        : <>
            <Table
                columns={columns}
                dataSource={books} />
            <ModalBookshelf
                open={open}
                setOpen={setOpen}
                modaldata={modaldata}
                setmodaldata={setmodaldata}/>
        </>;


    const props = {
        name: 'file',
        accept: 'application/json',
        action: '/fileUpload',
        //headers: {
        //    authorization: 'authorization-text',
        //},
        //beforeUpload: (file) => {
        //    const isTXT = file.type === 'json';
        //    if (!isTXT) {
        //        message.error(`${file.name} is not a txt file`);
        //    }
        //    return isTXT || Upload.LIST_IGNORE;
        //},
        onChange(info) {
            if (info.file.status !== 'uploading') {
                console.log(info.file, info.fileList);
            }
            if (info.file.status === 'done') {
                message.success(`${info.file.name} file uploaded successfully`);
            } else if (info.file.status === 'error') {
                message.error(`${info.file.name} file upload failed.`);
            }
        },
    };

    return (
        <div>
            <Upload {...props}>
                <Button icon={<UploadOutlined />}>Click to Upload</Button>
            </Upload>

            <h1 id="tabelLabel">My Bookshelf</h1>
            <p>This component demonstrates fetching data from the server.</p>
            {contents}
        </div>
    );

    async function populateBookData() {
        try {
            const response = await fetch('/bookshelf');
            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }
            const data = await response.json();

            // Assuming each book has 'novelTags' as a comma-separated string, split it into an array
            const booksWithDataTransformed = data.map(book => ({
                ...book,
                novelTags: book.novelTags.split(/(?:,| )+/).map(tag => tag.trim()),
                novelYear: new Date(book.novelDate).getFullYear().toString(),                
            }));

            setBooks(booksWithDataTransformed);
        } catch (error) {
            console.error('Failed to fetch books:', error);
        }
    }

}

export default Bookshelf;
